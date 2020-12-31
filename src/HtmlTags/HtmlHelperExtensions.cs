using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using HtmlTags.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HtmlTags
{
    using System;
    using System.Linq.Expressions;
    using Conventions;
    using Conventions.Elements;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.DependencyInjection;

    public static class HtmlHelperExtensions
    {
        private static readonly DotNotationElementNamingConvention NamingConvention = new();

        public static HtmlTag Input<T, TResult>(this IHtmlHelper<T> helper, Expression<Func<T, TResult>> expression)
            where T : class
        {
            var generator = GetGenerator(helper, expression);
            return generator.InputFor(expression);
        }

        public static HtmlTag ValidationMessage<T, TResult>(this IHtmlHelper<T> helper, Expression<Func<T, TResult>> expression)
            where T : class
        {
            var generator = GetGenerator(helper, expression);
            return generator.ValidationMessageFor(expression);
        }

        public static HtmlTag Label<T, TResult>(this IHtmlHelper<T> helper, Expression<Func<T, TResult>> expression)
            where T : class
        {
            var generator = GetGenerator(helper, expression);
            return generator.LabelFor(expression);
        }

        public static HtmlTag Display<T, TResult>(this IHtmlHelper<T> helper, Expression<Func<T, TResult>> expression)
            where T : class
        {
            var generator = GetGenerator(helper, expression);
            return generator.DisplayFor(expression);
        }

        public static HtmlTag Tag<T, TResult>(this IHtmlHelper<T> helper, Expression<Func<T, TResult>> expression, string category)
            where T : class
        {
            var generator = GetGenerator(helper, expression);
            return generator.TagFor(expression, category);
        }

        public static IElementGenerator<T> GetGenerator<T, TResult>(IHtmlHelper<T> helper, Expression<Func<T, TResult>> expression) where T : class
        {
            var modelExplorer = FromLambdaExpression(expression, helper.ViewData, helper.MetadataProvider);

            var elementName = new ElementName(NamingConvention.GetName(typeof(T), expression.ToAccessor()));

            return GetGenerator(helper, modelExplorer, helper.ViewContext, elementName);
        }

        public static IElementGenerator<T> GetGenerator<T>(IHtmlHelper<T> helper, params object[] additionalServices) where T : class
        {
            var library = helper.ViewContext.HttpContext.RequestServices.GetService<HtmlConventionLibrary>();
            object ServiceLocator(Type t) => additionalServices.FirstOrDefault(t.IsInstanceOfType) ?? helper.ViewContext.HttpContext.RequestServices.GetService(t);
            return ElementGenerator<T>.For(library, ServiceLocator, helper.ViewData.Model);
        }

        // from https://github.com/aspnet/AspNetCore/blob/v3.0.0/src/Mvc/Mvc.ViewFeatures/src/ExpressionMetadataProvider.cs#L15
        private static ModelExplorer FromLambdaExpression<TModel, TResult>(
            Expression<Func<TModel, TResult>> expression,
            ViewDataDictionary<TModel> viewData,
            IModelMetadataProvider metadataProvider)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            string propertyName = null;
            Type containerType = null;
            var legalExpression = false;

            // Need to verify the expression is valid; it needs to at least end in something
            // that we can convert to a meaningful string for model binding purposes

            switch (expression.Body.NodeType)
            {
                case ExpressionType.ArrayIndex:
                    // ArrayIndex always means a single-dimensional indexer;
                    // multi-dimensional indexer is a method call to Get().
                    legalExpression = true;
                    break;

                case ExpressionType.Call:
                    // Only legal method call is a single argument indexer/DefaultMember call
                    legalExpression = IsSingleArgumentIndexer(expression.Body);
                    break;

                case ExpressionType.MemberAccess:
                    // Property/field access is always legal
                    var memberExpression = (MemberExpression)expression.Body;
                    propertyName = memberExpression.Member is PropertyInfo ? memberExpression.Member.Name : null;
                    if (string.Equals(propertyName, "Model", StringComparison.Ordinal) &&
                        memberExpression.Type == typeof(TModel) &&
                        memberExpression.Expression.NodeType == ExpressionType.Constant)
                    {
                        // Special case the Model property in RazorPage<TModel>. (m => Model) should behave identically
                        // to (m => m). But do the more complicated thing for (m => m.Model) since that is a slightly
                        // different beast.)
                        return FromModel(viewData, metadataProvider);
                    }

                    // memberExpression.Expression can be null when this is a static field or property.
                    //
                    // This can be the case if the expression is like (m => Person.Name) where Name is a static field
                    // or property on the Person type.
                    containerType = memberExpression.Expression?.Type;

                    legalExpression = true;
                    break;

                case ExpressionType.Parameter:
                    // Parameter expression means "model => model", so we delegate to FromModel
                    return FromModel(viewData, metadataProvider);
            }

            if (!legalExpression)
            {
                throw new InvalidOperationException("Templates can be used only with field access, property access, single-dimension array index, or single-parameter custom indexer expressions.");
            }

            object modelAccessor(object container)
            {
                var model = (TModel)container;
                var cachedFunc = CachedExpressionCompiler.Process(expression);
                if (cachedFunc != null)
                {
                    return cachedFunc(model);
                }

                var func = expression.Compile();
                try
                {
                    return func(model);
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }

            ModelMetadata metadata = null;
            if (containerType != null && propertyName != null)
            {
                // Ex:
                //    m => m.Color (simple property access)
                //    m => m.Color.Red (nested property access)
                //    m => m.Widgets[0].Size (expression ending with property-access)
                metadata = metadataProvider.GetMetadataForType(containerType).Properties[propertyName];
            }

            if (metadata == null)
            {
                // Ex:
                //    m => 5 (arbitrary expression)
                //    m => foo (arbitrary expression)
                //    m => m.Widgets[0] (expression ending with non-property-access)
                //
                // This can also happen for any case where we cannot retrieve a model metadata.
                // This will happen for:
                // - fields
                // - statics
                // - non-visibility (internal/private)
                metadata = metadataProvider.GetMetadataForType(typeof(TResult));
                Debug.Assert(metadata != null);
            }

            return viewData.ModelExplorer.GetExplorerForExpression(metadata, modelAccessor);
        }

        // from https://github.com/aspnet/AspNetCore/blob/v3.0.0/src/Mvc/Mvc.ViewFeatures/src/ExpressionHelper.cs#L253
        private static bool IsSingleArgumentIndexer(Expression expression)
        {
            if (!(expression is MethodCallExpression methodExpression) || methodExpression.Arguments.Count != 1)
            {
                return false;
            }

            // Check whether GetDefaultMembers() (if present in CoreCLR) would return a member of this type. Compiler
            // names the indexer property, if any, in a generated [DefaultMember] attribute for the containing type.
            var declaringType = methodExpression.Method.DeclaringType;
            var defaultMember = declaringType.GetTypeInfo().GetCustomAttribute<DefaultMemberAttribute>(inherit: true);
            if (defaultMember == null)
            {
                return false;
            }

            // Find default property (the indexer) and confirm its getter is the method in this expression.
            var runtimeProperties = declaringType.GetRuntimeProperties();
            foreach (var property in runtimeProperties)
            {
                if ((string.Equals(defaultMember.MemberName, property.Name, StringComparison.Ordinal) &&
                     property.GetMethod == methodExpression.Method))
                {
                    return true;
                }
            }

            return false;
        }

        // from https://github.com/aspnet/AspNetCore/blob/v3.0.0/src/Mvc/Mvc.ViewFeatures/src/ExpressionMetadataProvider.cs#L206
        private static ModelExplorer FromModel(
            ViewDataDictionary viewData,
            IModelMetadataProvider metadataProvider)
        {
            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            if (viewData.ModelMetadata.ModelType == typeof(object))
            {
                // Use common simple type rather than object so e.g. Editor() at least generates a TextBox.
                var model = viewData.Model == null ? null : Convert.ToString(viewData.Model, CultureInfo.CurrentCulture);
                return metadataProvider.GetModelExplorerForType(typeof(string), model);
            }
            else
            {
                return viewData.ModelExplorer;
            }
        }
    }
}
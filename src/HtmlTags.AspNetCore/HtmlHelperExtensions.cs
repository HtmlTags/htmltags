using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

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
        public static HtmlTag Input<T, TResult>(this IHtmlHelper<T> helper, Expression<Func<T, TResult>> expression)
            where T : class
        {
            var generator = GetGenerator(helper, expression);
            return generator.InputFor(expression);
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
            var modelExplorer = 
                ExpressionMetadataProvider.FromLambdaExpression(expression, helper.ViewData, helper.MetadataProvider);

            return GetGenerator(helper, modelExplorer);
        }

        public static IElementGenerator<T> GetGenerator<T>(IHtmlHelper<T> helper, params object[] additionalServices) where T : class
        {
            var library = helper.ViewContext.HttpContext.RequestServices.GetService<HtmlConventionLibrary>();
            object ServiceLocator(Type t) => additionalServices.FirstOrDefault(t.IsInstanceOfType) ?? helper.ViewContext.HttpContext.RequestServices.GetService(t);
            return ElementGenerator<T>.For(library, ServiceLocator, helper.ViewData.Model);
        }
    }
}
namespace HtmlTags.Conventions
{
    using System;
    using System.Linq.Expressions;
    using Elements;
    using Reflection;

    public class ElementGenerator<T> : IElementGenerator<T> where T : class
    {
        private readonly ITagGenerator _tags;
        private Lazy<T> _model;

        private ElementGenerator(ITagGenerator tags)
        {
            _tags = tags;
        }

        public static ElementGenerator<T> For(HtmlConventionLibrary library, Func<Type, object> serviceLocator = null)
        {
            serviceLocator = serviceLocator ?? (Activator.CreateInstance);

            var tags = new TagGenerator(library.TagLibrary, new ActiveProfile(), serviceLocator);

            return new ElementGenerator<T>(tags);
        }

        public HtmlTag LabelFor(Expression<Func<T, object>> expression, string profile = null, T model = null)
        {
            return build(expression, ElementConstants.Label, profile, model);
        }

        public HtmlTag InputFor(Expression<Func<T, object>> expression, string profile = null, T model = null)
        {
            return build(expression, ElementConstants.Editor, profile, model);
        }

        public HtmlTag DisplayFor(Expression<Func<T, object>> expression, string profile = null, T model = null)
        {
            return build(expression, ElementConstants.Display, profile, model);
        }

        public T Model
        {
            get { return _model.Value; }
            set { _model = new Lazy<T>(() => value); }
        }

        public ElementRequest GetRequest(Expression<Func<T, object>> expression, T model = null)
        {
            return new ElementRequest(expression.ToAccessor())
            {
                Model = model ?? Model
            };
        }

        private HtmlTag build(Expression<Func<T, object>> expression, string category, string profile = null, T model = null)
        {
            ElementRequest request = GetRequest(expression, model);
            return _tags.Build(request, category, profile);
        }

        private HtmlTag build(ElementRequest request, string category, string profile = null, T model = null)
        {
            request.Model = model ?? Model;
            return _tags.Build(request, category, profile: profile);
        }

        // Below methods are tested through the IFubuPage.Show/Edit method tests
        public HtmlTag LabelFor(ElementRequest request, string profile = null, T model = null)
        {
            return build(request, ElementConstants.Label, profile, model);
        }

        public HtmlTag InputFor(ElementRequest request, string profile = null, T model = null)
        {
            return build(request, ElementConstants.Editor, profile, model);
        }

        public HtmlTag DisplayFor(ElementRequest request, string profile = null, T model = null)
        {
            return build(request, ElementConstants.Display, profile, model);
        }
    }
}
namespace HtmlTags.Conventions.Elements.Builders
{
    using System;

    // Tested through HtmlConventionRegistry
    public class ConditionalElementBuilder : IElementBuilderPolicy, ITagBuilder
    {
        private readonly Func<ElementRequest, bool> _filter;
        private readonly IElementBuilder _inner;

        public ConditionalElementBuilder(Func<ElementRequest, bool> filter, IElementBuilder inner)
        {
            _filter = filter;
            _inner = inner;
        }

        public string ConditionDescription { get; set; }
        public bool Matches(ElementRequest subject) => _filter(subject);

        public ITagBuilder BuilderFor(ElementRequest subject) => this;

        public HtmlTag Build(ElementRequest request) => _inner.Build(request);
    }
}
namespace HtmlTags.Conventions.Elements.Builders
{
    using System;

    // Tested through HtmlConventionRegistry
    public class ConditionalElementModifier : IElementModifier
    {
        private readonly Func<ElementRequest, bool> _filter;
        private readonly IElementModifier _inner;

        public ConditionalElementModifier(Func<ElementRequest, bool> filter, IElementModifier inner)
        {
            _filter = filter;
            _inner = inner;
        }

        public string ConditionDescription { get; set; }



        public bool Matches(ElementRequest token) => _filter(token);

        public void Modify(ElementRequest request) => _inner.Modify(request);
    }
}
using System;

namespace HtmlTags.Conventions
{
    public class LambdaTagBuilder : ITagBuilder
    {
        private readonly Func<ElementRequest, HtmlTag> _build;

        public LambdaTagBuilder(Func<ElementRequest, HtmlTag> build)
        {
            _build = build;
        }

        public HtmlTag Build(ElementRequest request) => _build(request);
    }
}
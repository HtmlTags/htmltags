using System;

namespace HtmlTags.Conventions
{
    public class LambdaTagBuilder<T> : ITagBuilder<T> where T : TagRequest
    {
        private readonly Func<T, HtmlTag> _build;

        public LambdaTagBuilder(Func<T, HtmlTag> build)
        {
            _build = build;
        }

        public HtmlTag Build(T request)
        {
            return _build(request);
        }
    }
}
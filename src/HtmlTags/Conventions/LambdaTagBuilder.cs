using System;

namespace HtmlTags.Conventions
{
    public class LambdaTagBuilder<T> : ITagBuilder<T> where T : TagRequest
    {
        private readonly Func<T, bool> _matcher;
        private readonly Func<T, HtmlTag> _build;

        public LambdaTagBuilder(Func<T, bool> matcher, Func<T, HtmlTag> build)
        {
            _matcher = matcher;
            _build = build;
        }

        public LambdaTagBuilder(Func<T, HtmlTag> build)
            : this(x => true, build)
        {

        }

        public bool Matches(T subject)
        {
            return _matcher(subject);
        }

        public HtmlTag Build(T request)
        {
            return _build(request);
        }
    }
}
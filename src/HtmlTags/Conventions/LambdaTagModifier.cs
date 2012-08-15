using System;

namespace HtmlTags.Conventions
{
    public class LambdaTagModifier<T> : ITagModifier<T> where T : TagRequest
    {
        private readonly Func<T, bool> _matcher;
        private readonly Action<T> _modify;

        public LambdaTagModifier(Func<T, bool> matcher, Action<T> modify)
        {
            _matcher = matcher;
            _modify = modify;
        }

        public LambdaTagModifier(Action<T> modify) : this(x => true, modify)
        {
        }

        public bool Matches(T token)
        {
            return _matcher(token);
        }

        public void Modify(T request)
        {
            _modify(request);
        }
    }
}
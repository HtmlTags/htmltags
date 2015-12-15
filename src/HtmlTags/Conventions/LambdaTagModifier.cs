using System;

namespace HtmlTags.Conventions
{
    public class LambdaTagModifier : ITagModifier
    {
        private readonly Func<ElementRequest, bool> _matcher;
        private readonly Action<ElementRequest> _modify;

        public LambdaTagModifier(Func<ElementRequest, bool> matcher, Action<ElementRequest> modify)
        {
            _matcher = matcher;
            _modify = modify;
        }

        public LambdaTagModifier(Action<ElementRequest> modify)
            : this(x => true, modify)
        {
        }

        public bool Matches(ElementRequest token) => _matcher(token);

        public void Modify(ElementRequest request) => _modify(request);
    }
}
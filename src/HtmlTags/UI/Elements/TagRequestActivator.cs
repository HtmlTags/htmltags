namespace HtmlTags.UI.Elements
{
    using System;
    using Conventions;

    public abstract class TagRequestActivator<T> : ITagRequestActivator where T : TagRequest
    {
        // TODO -- move this to HtmlTags

        public bool Matches(Type requestType)
        {
            return requestType.CanBeCastTo<T>();
        }

        public void Activate(TagRequest request)
        {
            Activate(request.As<T>());
        }

        public abstract void Activate(T request);
    }
}
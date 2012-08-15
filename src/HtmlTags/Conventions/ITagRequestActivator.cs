using System;

namespace HtmlTags.Conventions
{
    public interface ITagRequestActivator
    {
        bool Matches(Type requestType);
        void Activate(TagRequest request);
    }
}
using HtmlTags.UI.Elements.Builders;
using HtmlTags.Conventions;

namespace HtmlTags.UI.Elements
{
    public interface IElementTagOverride
    {
        string Category { get; } 
        string Profile { get; }
        IElementBuilder Builder();
    }

    public class ElementTagOverride<T> : IElementTagOverride where T : IElementBuilder, new()
    {
        public ElementTagOverride(string category, string profile)
        {
            Category = category ?? TagConstants.Default;
            Profile = profile ?? TagConstants.Default;
        }

        public string Category { get; private set; }
        public string Profile { get; private set; }
        public IElementBuilder Builder()
        {
            return new T();
        }
    }
}
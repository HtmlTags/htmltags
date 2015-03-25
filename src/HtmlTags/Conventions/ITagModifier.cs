namespace HtmlTags.Conventions
{
    public interface ITagModifier
    {
        bool Matches(ElementRequest token);
        void Modify(ElementRequest request);
    }
}
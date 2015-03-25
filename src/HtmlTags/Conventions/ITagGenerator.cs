namespace HtmlTags.Conventions
{
    public interface ITagGenerator
    {
        HtmlTag Build(ElementRequest request, string category = null, string profile = null);
        string ActiveProfile { get; }
    }
}
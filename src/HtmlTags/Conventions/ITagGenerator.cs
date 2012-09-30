namespace HtmlTags.Conventions
{
    public interface ITagGenerator<T> where T : TagRequest
    {
        HtmlTag Build(T request, string category = null, string profile = null);
        string ActiveProfile { get; }
    }
}
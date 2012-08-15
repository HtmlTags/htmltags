namespace HtmlTags.Conventions
{
    public interface ITagGenerator<T> where T : TagRequest
    {
        HtmlTag Build(T request);
        HtmlTag Build(string category, T request);

        string ActiveProfile { get; set;}
    }
}
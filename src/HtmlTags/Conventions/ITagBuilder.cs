namespace HtmlTags.Conventions
{
    public interface ITagBuilder<T> where T : TagRequest
    {
        bool Matches(T subject);
        HtmlTag Build(T request);
    }
}
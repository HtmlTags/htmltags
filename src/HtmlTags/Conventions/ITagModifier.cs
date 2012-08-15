namespace HtmlTags.Conventions
{
    public interface ITagModifier<T> where T : TagRequest
    {
        bool Matches(T token);
        void Modify(T request);
    }
}
namespace HtmlTags.Conventions
{
    public interface ITagLibraryVisitor<T> where T : TagRequest
    {
        void Category(string name, TagCategory<T> category);
        void BuilderSet(string profile, BuilderSet<T> builders);
    }
}
namespace HtmlTags.Conventions
{
    public interface ITagLibraryVisitor
    {
        void Category(string name, TagCategory category);
        void BuilderSet(string profile, BuilderSet builders);
    }
}
namespace HtmlTags.Conventions
{
    public interface IHtmlConventionVisitor
    {
        ITagLibraryVisitor VisitorFor();
    }
}
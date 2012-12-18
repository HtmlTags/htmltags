namespace HtmlTags.Conventions
{
    public interface IHtmlConventionVisitor
    {
        ITagLibraryVisitor<T> VisitorFor<T>() where T : TagRequest;
    }
}
namespace HtmlTags.Conventions
{
    public interface IVisitable
    {
        void AcceptVisitor(IHtmlConventionVisitor visitor);
    }
}
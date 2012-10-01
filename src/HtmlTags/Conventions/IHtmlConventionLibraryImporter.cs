namespace HtmlTags.Conventions
{
    public interface IHtmlConventionLibraryImporter
    {
        void Import(HtmlConventionLibrary target, HtmlConventionLibrary source);
    }
}
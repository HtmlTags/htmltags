namespace HtmlTags.Conventions
{
    public class HtmlConventionLibraryImporter : IHtmlConventionLibraryImporter
    {
        public void Import(HtmlConventionLibrary target, HtmlConventionLibrary source)
        {
            target.For().Import(source.For());
        }
    }
}
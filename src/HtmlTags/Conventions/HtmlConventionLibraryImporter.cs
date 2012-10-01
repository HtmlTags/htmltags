namespace HtmlTags.Conventions
{
    public class HtmlConventionLibraryImporter<T> : IHtmlConventionLibraryImporter where T : TagRequest
    {
        public void Import(HtmlConventionLibrary target, HtmlConventionLibrary source)
        {
            target.For<T>().Import(source.For<T>());
        }
    }
}
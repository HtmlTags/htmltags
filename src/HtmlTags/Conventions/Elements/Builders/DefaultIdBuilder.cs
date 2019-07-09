using System.Text.RegularExpressions;

namespace HtmlTags.Conventions.Elements.Builders
{
    public class DefaultIdBuilder
    {
        private static readonly Regex IdRegex = new Regex(@"[\.\[\]]");

        public static string Build(ElementRequest request) 
            => IdRegex.Replace(request.ElementId, "_");
    }
}
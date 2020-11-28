namespace HtmlTags.Conventions.Elements.Builders
{
    using System.Linq;
    using System.Text.RegularExpressions;

    public class DefaultLabelBuilder : IElementBuilder
    {
        private static readonly Regex[] RxPatterns =
        {
            new Regex("([a-z])([A-Z])", RegexOptions.IgnorePatternWhitespace),
            new Regex("([0-9])([a-zA-Z])", RegexOptions.IgnorePatternWhitespace),
            new Regex("([a-zA-Z])([0-9])", RegexOptions.IgnorePatternWhitespace)
        };

        public bool Matches(ElementRequest subject) => true;

        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("label").Attr("for", DefaultIdBuilder.Build(request)).Text(BreakUpCamelCase(request.Accessor.Name));   
        }

        public static string BreakUpCamelCase(string fieldName)
        {
            var output = RxPatterns.Aggregate(fieldName,
                (current, regex) => regex.Replace(current, "$1 $2"));
            return output.Replace('_', ' ');
        }
    }
}
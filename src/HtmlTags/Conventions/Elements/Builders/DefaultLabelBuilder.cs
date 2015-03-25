namespace HtmlTags.Conventions.Elements.Builders
{
    using System.ComponentModel;
    using System.Linq;
    using System.Text.RegularExpressions;

    [Description("Builds a <label> element using the accessor name as the text and sets @for to the display id")]
    public class DefaultLabelBuilder : IElementBuilder
    {
        public bool Matches(ElementRequest subject)
        {
            return true;
        }

        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("label").Attr("for", request.ElementId).Text(BreakUpCamelCase(request.Accessor.Name));   
        }

        public static string BreakUpCamelCase(string fieldName)
        {
            var patterns = new[]
                {
                    "([a-z])([A-Z])",
                    "([0-9])([a-zA-Z])",
                    "([a-zA-Z])([0-9])"
                };
            var output = patterns.Aggregate(fieldName,
                (current, pattern) => Regex.Replace(current, pattern, "$1 $2", RegexOptions.IgnorePatternWhitespace));
            return output.Replace('_', ' ');
        }
    }
}
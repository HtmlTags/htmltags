using System.Text.RegularExpressions;

namespace HtmlTags
{
    public static class CssClassNameValidator
    {
        public const string DefaultClass = "cssclassnamevalidator-santized";

        public static bool IsJsonClassName(string className)
        {
            return className.StartsWith("{") && className.EndsWith("}")
                    || className.StartsWith("[") && className.EndsWith("]");
        }

        public static bool IsValidClassName(string className)
        {
            return IsJsonClassName(className) || Regex.IsMatch(className, @"^-?[_a-zA-Z]+[_a-zA-Z0-9-]*$");
        }
    }
}
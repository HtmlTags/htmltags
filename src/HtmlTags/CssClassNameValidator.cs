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

        public static string SanitizeClassName(string className)
        {
            if (string.IsNullOrEmpty(className)) return DefaultClass;

            if (IsValidClassName(className)) return className;

            // it can't have anything other than _,-,a-z,A-Z, or 0-9
            className = Regex.Replace(className, "[^_a-zA-Z0-9-]", "");

            // Strip invalid leading combinations (i.e. '-9test' -> 'test')
            // if it starts with '-', it must be followed by _, a-z, A-Z
            className = Regex.Replace(className, "^-?[^_a-zA-Z]+(?<rest>.*)$", @"${rest}");

            // if the whole thing was invalid, we'll end up with an empty string. That's not valid either, so return the default
            return string.IsNullOrEmpty(className) ? DefaultClass : className;
        }
    }
}
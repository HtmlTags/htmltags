using System.Text.RegularExpressions;

namespace HtmlTags
{
    public static class CssClassNameValidator
    {
        public const string DefaultClass = "cssclassnamevalidator-santized";
        public const string ValidStartRegex = @"^-?[_a-zA-Z]+";
        public const string InvalidStartRegex = @"^-?[^_a-zA-Z]+";
        public const string ValidClassChars = @"_a-zA-Z0-9-";

        public static bool AllowInvalidCssClassNames { get; set; }

        public static bool IsJsonClassName(string className)
        {
            return className.StartsWith("{") && className.EndsWith("}")
                    || className.StartsWith("[") && className.EndsWith("]");
        }

        public static bool IsValidClassName(string className)
        {
            var pattern = string.Format(@"{0}[{1}]*$", ValidStartRegex, ValidClassChars);
            return AllowInvalidCssClassNames || IsJsonClassName(className) || Regex.IsMatch(className, pattern);
        }

        public static string SanitizeClassName(string className)
        {
            if (string.IsNullOrEmpty(className)) return DefaultClass;

            if (IsValidClassName(className)) return className;

            // it can't have anything other than _,-,a-z,A-Z, or 0-9
            var pattern = string.Format("[^{0}]", ValidClassChars);
            className = Regex.Replace(className, pattern, "");

            // Strip invalid leading combinations (i.e. '-9test' -> 'test')
            // if it starts with '-', it must be followed by _, a-z, A-Z
            var invalidleadingpattern = string.Format("{0}(?<rest>.*)$", InvalidStartRegex);
            className = Regex.Replace(className, invalidleadingpattern, @"${rest}");

            // if the whole thing was invalid, we'll end up with an empty string. That's not valid either, so return the default
            return string.IsNullOrEmpty(className) ? DefaultClass : className;
        }
    }
}
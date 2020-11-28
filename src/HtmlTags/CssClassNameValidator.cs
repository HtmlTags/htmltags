using System.Text.RegularExpressions;

namespace HtmlTags
{
    public static class CssClassNameValidator
    {
        public const string DefaultClass = "cssclassnamevalidator-santized";
        public const string ValidStartRegex = @"^-?[_a-zA-Z]+";
        public const string InvalidStartRegex = @"^-?[^_a-zA-Z]+";
        public const string ValidClassChars = @"_a-zA-Z0-9-";
        private static readonly Regex RxValidClassName = new Regex($@"{ValidStartRegex}[{ValidClassChars}]*$");
        private static readonly Regex RxReplaceInvalidChars = new Regex($"[^{ValidClassChars}]");
        private static readonly Regex RxReplaceLeadingChars = new Regex($"{InvalidStartRegex}(?<rest>.*)$");

        public static bool AllowInvalidCssClassNames { get; set; }

        public static bool IsJsonClassName(string className)
        {
            return className.StartsWith("{") && className.EndsWith("}")
                    || className.StartsWith("[") && className.EndsWith("]");
        }

        public static bool IsValidClassName(string className)
        {
            return AllowInvalidCssClassNames || IsJsonClassName(className) || RxValidClassName.IsMatch(className);
        }

        public static string SanitizeClassName(string className)
        {
            if (string.IsNullOrEmpty(className)) return DefaultClass;

            if (IsValidClassName(className)) return className;

            // it can't have anything other than _,-,a-z,A-Z, or 0-9
            className = RxReplaceInvalidChars.Replace(className, "");

            // Strip invalid leading combinations (i.e. '-9test' -> 'test')
            // if it starts with '-', it must be followed by _, a-z, A-Z
            className = RxReplaceLeadingChars.Replace(className, @"${rest}");

            // if the whole thing was invalid, we'll end up with an empty string. That's not valid either, so return the default
            return string.IsNullOrEmpty(className) ? DefaultClass : className;
        }
    }
}
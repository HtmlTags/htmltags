namespace HtmlTags.Conventions.Formatting
{
    using System;

    public class StringifierStrategy
    {
        public Func<GetStringRequest, bool> Matches;
        public Func<GetStringRequest, string> StringFunction;
    }
}
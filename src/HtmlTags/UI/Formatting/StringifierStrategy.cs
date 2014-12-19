using System;

namespace HtmlTags.UI.Formatting
{
    public class StringifierStrategy
    {
        public Func<GetStringRequest, bool> Matches;
        public Func<GetStringRequest, string> StringFunction;
    }
}
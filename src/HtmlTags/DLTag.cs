using System;

namespace HtmlTags
{
    public class DLTag : HtmlTag
    {
        public DLTag()
            : base("dl")
        {
        }

        public DLTag(Action<DLTag> configure)
            : this()
        {
            configure(this);
        }

        public DLTag AddDefinition(string header, string content)
        {
            Add("dt").Text(header);
            Add("dl").Text(content);

            return this;
        }
    }
}
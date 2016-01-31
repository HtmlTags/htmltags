using System;

namespace HtmlTags
{
    public class LabelTag : HtmlTag
    {
        public LabelTag() : base("label")
        {
        }

        public LabelTag(Action<HtmlTag> configure) : base("label", configure)
        {
        }

        public LabelTag(HtmlTag parent) : base("label", parent)
        {
        }

        public LabelTag(string tagId, string text) : this()
        {
            Attr("for", tagId).Text(text);
        }

        public HtmlTag For(string tagId)
        {
            return this.Attr("for", tagId);
        }
    }
}
using System.Diagnostics;
using System.IO;
using System.Web.UI;

namespace StoryTeller.Rendering.Html
{
    public class SelfClosingHtmlWriter : HtmlTextWriter
    {
        private int unclosedTags;

        public SelfClosingHtmlWriter(TextWriter writer, string tabString)
            : base(writer, tabString)
        {
            unclosedTags = 0;
        }

        public override void RenderBeginTag(string tagName)
        {
            ++unclosedTags;
            base.RenderBeginTag(tagName);
        }

        public override void RenderEndTag()
        {
            base.RenderEndTag();
            --unclosedTags;
        }

        public override void EndRender()
        {
            while (unclosedTags > 0)
            {
                int lastUnclosedTagsCount = unclosedTags;
                RenderEndTag();
                Debug.Assert(unclosedTags < lastUnclosedTagsCount);
            }
            base.EndRender();
        }
    }
}
using System;
using System.Linq;

namespace HtmlTags
{
    public class SelectTag : HtmlTag
    {
        public SelectTag()
            : base("select")
        {
        }

        public SelectTag(Action<SelectTag> configure) : this()
        {
            configure(this);
        }

        public HtmlTag Option(string display, object value)
        {
            return Add("option").Text(display).Attr("value", value);
        }

        public void SelectByValue(object value)
        {
            var child = Children.FirstOrDefault(x => x.Attr("value").Equals(value));
            if (child != null)
            {
                child.Attr("selected", "selected");
            }
        }
    }
}
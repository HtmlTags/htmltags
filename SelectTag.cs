namespace HtmlTags
{
    public class SelectTag : HtmlTag
    {
        public SelectTag()
            : base("select")
        {
        }

        public HtmlTag Option(string display, object value)
        {
            return Add("option").Text(display).Attr("value", value);
        }
    }
}
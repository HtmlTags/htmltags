namespace HtmlTags
{
    public class HiddenTag : HtmlTag
    {
        public HiddenTag()
            : base("input")
        {
            Attr("type", "hidden");
        }
    }
}
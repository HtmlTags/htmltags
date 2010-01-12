namespace HtmlTags
{
    public class TextboxTag : HtmlTag
    {
        public TextboxTag()
            : base("input")
        {
            Attr("type", "text");
        }
    }
}
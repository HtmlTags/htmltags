namespace HtmlTags
{
    public class FormTag : HtmlTag
    {
        public FormTag(string url) : this()
        {
            Action(url);
        }

        public FormTag() : base("form")
        {
            NoClosingTag();
            Id("mainForm");
            Attr("method", "post");
        }

        public FormTag Action(string url)
        {
            Attr("action", url);
            return this;
        }
    }
}
namespace HtmlTags
{
   public class RadionButtonTag : HtmlTag
    {
        public RadionButtonTag(string name, string value, bool isChecked = false) : base("input")
        {
            Attr("type", "radio")
                .Name(name)
                .IdFromName()
                .Value(value);
            if (isChecked)
            {
                this.Checked();
            }
        }
    }
}
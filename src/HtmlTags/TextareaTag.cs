using HtmlTags.Extended.Attributes;

namespace HtmlTags
{
    public class TextareaTag:TextboxTag
    {
        public TextareaTag()
        {
            this.MultilineMode();
        }

        public TextareaTag(string name,string value):base(name,value)
        {
            this.MultilineMode();
        }

        public TextareaTag Rows(int rows)
        {
            this.Attr("rows", rows);
            return this;
        }
        
        public TextareaTag Cols(int cols)
        {
            this.Attr("cols", cols);
            return this;
        }


    }
}
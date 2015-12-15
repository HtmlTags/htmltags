using System;
using System.Linq;

namespace HtmlTags
{
    public class TableTag : HtmlTag
    {
        public HtmlTag THead { get; }

        public HtmlTag TBody { get; }

        public HtmlTag TFoot { get; }

        public TableTag()
            : base("table")
        {
            THead = new HtmlTag("thead", this);
            TFoot = new HtmlTag("tfoot", this).Render(false);
            TBody = new HtmlTag("tbody", this);
        }

        public TableTag CaptionText(string text)
        {
            HtmlTag caption = ExistingCaption();
            if (caption == null)
            {
                caption = new HtmlTag("caption");
                Children.Insert(0, caption);
            }

            caption.Text(text);

            return this;
        }

        public string CaptionText()
        {
            var caption = ExistingCaption();
            return caption == null ? string.Empty : caption.Text();
        }

        private HtmlTag ExistingCaption() => Children.FirstOrDefault(x => x.TagName() == "caption");

        public TableRowTag AddHeaderRow() => THead.Add<TableRowTag>();

        public TableTag AddHeaderRow(Action<TableRowTag> configure)
        {
            configure(AddHeaderRow());

            return this;
        }

        public TableRowTag AddBodyRow() => TBody.Add<TableRowTag>();

        public TableTag AddBodyRow(Action<TableRowTag> configure)
        {
            configure(AddBodyRow());
            return this;
        }

        public TableTag AddFooterRow(Action<TableRowTag> configure)
        {
            TFoot.Render(true);
            configure(TFoot.Add<TableRowTag>());
            return this;
        }


        public TableTag Caption(string caption)
        {
            var captionTag = ExistingCaption();
            if (captionTag == null)
            {
                captionTag = new HtmlTag("caption");
                Children.Insert(0, captionTag);
            }

            captionTag.Text(caption);

            return this;
        }
    }

    public class TableRowTag : HtmlTag
    {
        public TableRowTag()
            : base("tr")
        {
        }

        public HtmlTag Header(string text) => new HtmlTag("th", this).Text(text);

        public HtmlTag Header() => new HtmlTag("th", this);

        public HtmlTag Cell(string text) => new HtmlTag("td", this).Text(text);

        public HtmlTag Cell() => new HtmlTag("td", this);
    }
}
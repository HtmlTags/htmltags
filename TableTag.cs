using System;

namespace HtmlTags
{
    public class TableTag : HtmlTag
    {
        private readonly HtmlTag _body;
        private readonly HtmlTag _header;

        public TableTag()
            : base("table")
        {
            _header = Add("thead");
            _body = Add("tbody");
        }

        public TableRowTag AddHeaderRow()
        {
            return _header.Child<TableRowTag>();
        }

        public TableTag AddHeaderRow(Action<TableRowTag> configure)
        {
            configure(AddHeaderRow());

            return this;
        }

        public TableRowTag AddBodyRow()
        {
            return _body.Child<TableRowTag>();
        }

        public TableTag AddBodyRow(Action<TableRowTag> configure)
        {
            configure(AddBodyRow());
            return this;
        }
    }

    public class TableRowTag : HtmlTag
    {
        public TableRowTag()
            : base("tr")
        {
        }

        public HtmlTag Header(string text)
        {
            return Add("th").Text(text);
        }

        public HtmlTag Header()
        {
            return Add("th");
        }

        public HtmlTag Cell(string text)
        {
            return Add("td").Text(text);
        }

        public HtmlTag Cell()
        {
            return Add("td");
        }
    }
}
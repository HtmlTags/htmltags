using Shouldly;
using Xunit;
using System.Linq;

namespace HtmlTags.Testing
{
    
    public class TableTagTester
    {
        [Fact]
        public void should_create_an_empty_table()
        {
            var expected = getExpectedHtml(null, null, null);
            new TableTag().ToString().ShouldBe(expected);
        }

        [Fact]
        public void should_add_headings_in_a_header_row_to_the_thead()
        {
            var expected = getExpectedHtml("<tr><th>Heading 1</th><th>Heading 2</th></tr>", null, null);

            var tag = new TableTag();
            var headerRow = tag.AddHeaderRow();
            headerRow.Header().Text("Heading 1");
            headerRow.Header("Heading 2");
            tag.ToString().ShouldBe(expected);
        }

        [Fact]
        public void header_row_can_be_configured_with_headings()
        {
            var expected = getExpectedHtml("<tr><th>Heading 1</th><th>Heading 2</th></tr>", null, null);
            new TableTag()
                .AddHeaderRow(h =>
                                  {
                                      h.Header("Heading 1");
                                      h.Header("Heading 2");
                                  }).ToString()
                .ShouldBe(expected);
        }

        [Fact]
        public void add_caption_always_puts_the_caption_at_top()
        {
            var tag = new TableTag();
            tag.CaptionText("some caption");

            tag.Children[0].Text().ShouldBe("some caption");
            tag.Children[0].TagName().ShouldBe("caption");


            tag.CaptionText().ShouldBe("some caption");

            tag.CaptionText("other caption");
            tag.CaptionText().ShouldBe("other caption");

            tag.Children.Count(x => x.TagName() == "caption").ShouldBe(1);
        }

        [Fact]
        public void caption_is_empty_string_if_hasnt_been_set()
        {
            new TableTag().CaptionText().ShouldBe(string.Empty);
        }

        [Fact]
        public void should_add_cells_to_rows_in_the_tbody()
        {
            var expected = getExpectedHtml(null,
                                           "<tr><td>cell 1</td><td>cell 2</td></tr><tr><td>cell 3</td><td>cell 4</td></tr>",
                                           null);

            var tag = new TableTag();
            var bodyRow1 = tag.AddBodyRow();
            bodyRow1.Cell().Text("cell 1");
            bodyRow1.Cell("cell 2");
            var bodyRow2 = tag.AddBodyRow();
            bodyRow2.Cell("cell 3");
            bodyRow2.Cell("cell 4");
            tag.ToString().ShouldBe(expected);
        }

        [Fact]
        public void body_rows_can_be_configured_with_cells()
        {
            var expected = getExpectedHtml(null,
                                           "<tr><td>cell 1</td><td>cell 2</td></tr><tr><td>cell 3</td><td>cell 4</td></tr>",
                                           null);
            new TableTag()
                .AddBodyRow(b =>
                                {
                                    b.Cell("cell 1");
                                    b.Cell("cell 2");
                                })
                .AddBodyRow(b =>
                                {
                                    b.Cell("cell 3");
                                    b.Cell("cell 4");
                                }).ToString()
                .ShouldBe(expected);
        }

        [Fact]
        public void should_add_a_footer_to_the_tfoot()
        {
            var expected = getExpectedHtml(null, null, "<tfoot><tr><td>footer</td></tr></tfoot>");
            new TableTag().AddFooterRow(f => f.Cell("footer")).ToString().ShouldBe(expected);
        }

        [Fact]
        public void should_build_a_complete_table()
        {
            var expected = getExpectedHtml("the caption",
                                           "<tr><th>heading 1</th><th>heading 2</th></tr>",
                                           "<tr><td>cell 1.1</td><td>cell 1.2</td></tr><tr><td>cell 2.1</td><td>cell 2.2</td></tr>",
                                           "<tfoot><tr><td>footer 1</td><td>footer 2</td></tr></tfoot>");
            new TableTag()
                .AddHeaderRow(h =>
                                  {
                                      h.Header("heading 1");
                                      h.Header("heading 2");
                                  })
                .AddFooterRow(f =>
                                  {
                                      f.Cell("footer 1");
                                      f.Cell("footer 2");
                                  })
                .AddBodyRow(b =>
                                {
                                    b.Cell("cell 1.1");
                                    b.Cell("cell 1.2");
                                })
                .AddBodyRow(b =>
                                {
                                    b.Cell("cell 2.1");
                                    b.Cell("cell 2.2");
                                })
                .Caption("the caption").ToString()
                .ShouldBe(expected);
        }

        [Fact]
        public void should_allow_multiple_rows_in_the_header()
        {
            var expected = getExpectedHtml("<tr><th>heading</th></tr><tr><td>explanation</td></tr>", null, null);
            new TableTag()
                .AddHeaderRow(h => h.Header("heading"))
                .AddHeaderRow(h => h.Cell("explanation")).ToString()
                .ShouldBe(expected);
        }

        [Fact]
        public void cells_can_have_colspan_and_rowspan()
        {
            var expected = getExpectedHtml(null,
                                           "<tr><td colspan=\"2\">spanning cell</td></tr><tr><td>cell 2</td><td>cell 3</td></tr><tr><td rowspan=\"2\">spanning cell</td></tr>",
                                           null);
            new TableTag()
                .AddBodyRow(b => b.Cell("spanning cell").Attr("colspan", 2))
                .AddBodyRow(b =>
                                {
                                    b.Cell("cell 2");
                                    b.Cell("cell 3");
                                })
                .AddBodyRow(b => b.Cell("spanning cell").Attr("rowspan", 2)).ToString()
                .ShouldBe(expected);
        }

        [Fact]
        public void check_thead_tbody_tfoot_access()
        {
            var expected = "<table><thead class=\"header\"></thead><tfoot class=\"footer\"></tfoot><tbody class=\"body\"></tbody></table>";

            var table = new TableTag();
            table.THead.AddClass("header");
            table.TBody.AddClass("body");
            table.TFoot.AddClass("footer").Render(true);

            System.Diagnostics.Trace.TraceInformation(table.ToString());
            table.ToString().ShouldBe(expected);
        }

        private static string getExpectedHtml(string theadContents, string tbodyContents, string tfoot)
        {
            return getExpectedHtml(null, theadContents, tbodyContents, tfoot);
        }

        private static string getExpectedHtml(string caption, string theadContents, string tbodyContents, string tfoot)
        {
            string captionTag = string.IsNullOrEmpty(caption)
                                    ? string.Empty
                                    : string.Format("<caption>{0}</caption>", caption);
            return string.Format("<table>{0}<thead>{1}</thead>{2}<tbody>{3}</tbody></table>",
                captionTag, theadContents, tfoot, tbodyContents);
        }
    }
}
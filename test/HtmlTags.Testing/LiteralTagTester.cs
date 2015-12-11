using Xunit;
using Should;

namespace HtmlTags.Testing
{
    
    public class LiteralTagTester
    {
        [Fact]
        public void just_writes_the_literal_html_it_is_given()
        {
            var html = "<div>I did this</div>";
            var tag = new LiteralTag(html);

            tag.ToString().ShouldEqual(html);
        }

        [Fact]
        public void literal_tag_hosted_inside_of_another_tag()
        {
            var html = "<div>I did this</div>";
            var tag = new LiteralTag(html);

            new HtmlTag("body").Append(tag)
                .ToString().ShouldEqual("<body>" + html + "</body>");
        }

        [Fact]
        public void literal_tag_hosted_inside_of_another_tag_from_the_extension_method()
        {
            var html = "<div>I did this</div>";

            new HtmlTag("body").AppendHtml(html)
                .ToString().ShouldEqual("<body>" + html + "</body>");
        }
    }
}
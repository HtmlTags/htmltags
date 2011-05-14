using NUnit.Framework;
using HtmlTags.Extended.TagBuilders;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class TagBuilderExtensionsTester
    {
        [Test]
        public void append_a_child_span_tag()
        {
            var tag = new HtmlTag("div").Span(x => x.Text("inner"));
            tag.ToString().ShouldEqual("<div><span>inner</span></div>");
        }

        [Test]
        public void append_a_child_div_tag()
        {
            var tag = new HtmlTag("body").Div(x => x.Id("inner"));
            tag.ToString().ShouldEqual("<body><div id=\"inner\"></div></body>");
        }

        [Test]
        public void create_and_return_a_link_as_a_child_of_another_tag()
        {
            var tag = new HtmlTag("div");
            var link = tag.ActionLink("click", "important", "invoke");
            link.ToString().ShouldEqual("<a href=\"#\" class=\"important invoke\">click</a>");
            tag.ToString().ShouldEqual("<div><a href=\"#\" class=\"important invoke\">click</a></div>");
        }
    }
}
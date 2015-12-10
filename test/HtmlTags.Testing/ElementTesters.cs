using FubuTestingSupport;
using NUnit.Framework;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class ElementTesters
    {
        [Test]
        public void create_a_div_with_an_id()
        {
            new DivTag("first-name").ToString().ShouldEqual("<div id=\"first-name\"></div>");
        }

        [Test]
        public void create_a_div_without_an_id()
        {
            new DivTag().ToString().ShouldEqual("<div></div>");
        }

        [Test]
        public void create_a_hidden_input()
        {
            new HiddenTag().ToString().ShouldEqual("<input type=\"hidden\" />");
        }

        [Test]
        public void create_a_text_input()
        {
            new TextboxTag().ToString().ShouldEqual("<input type=\"text\" />");
        }

        [Test]
        public void create_a_text_input_with_name_and_value()
        {
            var tag = new TextboxTag("firstname", "Lucas");
            tag.ToString().ShouldEqual("<input type=\"text\" name=\"firstname\" value=\"Lucas\" />");
        }
    }

    [TestFixture]
    public class FormTagTester
    {
        [Test]
        public void form_tag_creates_the_opening_element_of_a_form_with_id_mainForm()
        {
            var tag = new FormTag();
            tag.Attr("method").ShouldEqual("post");
        }

        [Test]
        public void form_id_can_be_customized()
        {
            var tag = new FormTag().Id("other-form");
            tag.Id().ShouldEqual("other-form");
        }

        [Test]
        public void form_method_can_be_customized()
        {
            var tag = new FormTag().Method("get");
            tag.Attr("method").ShouldEqual("get");
        }

        [Test]
        public void form_action_can_be_specified()
        {
            var tag = new FormTag().Action("/user/create");
            tag.Attr("action").ShouldEqual("/user/create");
        }

        [Test]
        public void form_action_can_be_specified_via_constructor()
        {
            var tag = new FormTag("/user/create");
            tag.Attr("action").ShouldEqual("/user/create");
        }

        [Test]
        public void form_tag_has_closing_tag_by_default()
        {
            new FormTag().HasClosingTag().ShouldBeTrue();
        }
    }

    [TestFixture]
    public class BrTagTester
    {
        [SetUp]
        public void Setup()
        {
            BrTag.ComplianceMode = BrTag.ComplianceModes.AspNet;
        }
        [Test]
        public void default_renders_with_open_close_tags()
        {
            var tag = new BrTag();
            tag.ToString().ShouldEqual("<br></br>");
        }

        [Test]
        public void xhtml_mode_renders_with_self_closing_tag()
        {
            BrTag.ComplianceMode = BrTag.ComplianceModes.Xhtml;
            var tag = new BrTag();
            tag.ToString().ShouldEqual("<br />");
        }

        [Test]
        public void html5_mode_renders_with_single_tag()
        {
            BrTag.ComplianceMode = BrTag.ComplianceModes.Html5;
            var tag = new BrTag();
            tag.ToString().ShouldEqual("<br>");
        }
    }
}
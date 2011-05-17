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
            tag.ToString().ShouldEqual("<form id=\"mainForm\" method=\"post\">");
        }

        [Test]
        public void form_id_can_be_customized()
        {
            var tag = new FormTag().Id("other-form");
            tag.ToString().ShouldEqual("<form id=\"other-form\" method=\"post\">");
        }

        [Test]
        public void form_method_can_be_customized()
        {
            var tag = new FormTag().Method("get");
            tag.ToString().ShouldEqual("<form id=\"mainForm\" method=\"get\">");
        }

        [Test]
        public void form_action_can_be_specified()
        {
            var tag = new FormTag().Action("/user/create");
            tag.ToString().ShouldEqual("<form id=\"mainForm\" method=\"post\" action=\"/user/create\">");
        }

        [Test]
        public void form_action_can_be_specified_via_constructor()
        {
            var tag = new FormTag("/user/create");
            tag.ToString().ShouldEqual("<form id=\"mainForm\" method=\"post\" action=\"/user/create\">");
        }

    }
}
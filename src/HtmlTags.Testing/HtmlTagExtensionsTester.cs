using NUnit.Framework;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class HtmlTagExtensionsTester
    {
        [Test]
        public void value_ext_method()
        {
            new HtmlTag("input").Value("the value")
                .ToString().ShouldEqual("<input value=\"the value\" />");
        }

        [Test]
        public void name_ext_method()
        {
            new HtmlTag("input").Name("the name")
                .Attr("name").ShouldEqual("the name");
        }

        [Test]
        public void autocomplete_ext_method()
        {
            new HtmlTag("div").NoAutoComplete()
                .Attr("autocomplete").ShouldEqual("off");
        }

        [Test]
        public void password_mode_ext_method()
        {
            new HtmlTag("a").Name("password").PasswordMode().ToString()
                .ShouldEqual("<input name=\"password\" type=\"password\" autocomplete=\"off\" />");
        }

        [Test]
        public void file_upload_mode_ext_method()
        {
            new HtmlTag("input").FileUploadMode().ToString()
                .ShouldEqual("<input type=\"file\" />");
        }

        [Test]
        public void hide_unless_negative_case()
        {
            new HtmlTag("div").HideUnless(true).HasStyle("display").ShouldBeFalse();
        }

        [Test]
        public void hide_unless_positive_case()
        {
            new HtmlTag("div").HideUnless(false).Style("display").ShouldEqual("none");
        }
    }



    [TestFixture]
    public class when_converting_an_input_tag_with_a_value_into_a_multiline_editor
    {
        private HtmlTag theTag;
        private string theOriginalValue = "something";

        [SetUp]
        public void SetUp()
        {
            theTag = new HtmlTag("input").Value(theOriginalValue).MultilineMode();
        }

        [Test]
        public void the_value_attribute_should_removed()
        {
            theTag.HasAttr("value").ShouldBeFalse();
        }

        [Test]
        public void the_previous_value_should_be_moved_to_the_text_attribute()
        {
            theTag.Text().ShouldEqual(theOriginalValue);
        }

        [Test]
        public void the_tag_name_should_be_changed_to_textarea()
        {
            theTag.TagName().ShouldEqual("textarea");
        }
    }

    [TestFixture]
    public class when_converting_a_tag_to_multiline_that_does_not_start_with_a_value
    {
        private HtmlTag theTag;

        [SetUp]
        public void SetUp()
        {
            theTag = new HtmlTag("input").MultilineMode();
        }

        [Test]
        public void the_tag_name_should_be_changed_to_textarea()
        {
            theTag.TagName().ShouldEqual("textarea");
        }
    }


    [TestFixture]
    public class MultilineModeTester
    {
        
    }
}
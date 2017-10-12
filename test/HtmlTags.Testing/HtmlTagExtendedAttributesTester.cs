using Shouldly;
using HtmlTags.Extended;
using HtmlTags.Extended.Attributes;
using Xunit;

namespace HtmlTags.Testing
{
    
    public class HtmlTagExtendedAttributesTester
    {
        [Fact]
        public void value_ext_method()
        {
            new HtmlTag("input").Value("the value")
                .ToString().ShouldBe("<input value=\"the value\" />");
        }

        [Fact]
        public void name_ext_method()
        {
            new HtmlTag("input").Name("the name")
                .Attr("name").ShouldBe("the name");
        }

        [Fact]
        public void autocomplete_ext_method()
        {
            new HtmlTag("div").NoAutoComplete()
                .Attr("autocomplete").ShouldBe("off");
        }

        [Fact]
        public void password_mode_ext_method()
        {
            new HtmlTag("a").Name("password").PasswordMode().ToString()
                .ShouldBe("<input name=\"password\" type=\"password\" autocomplete=\"off\" />");
        }

        [Fact]
        public void file_upload_mode_ext_method()
        {
            new HtmlTag("input").FileUploadMode().ToString()
                .ShouldBe("<input type=\"file\" />");
        }

        [Fact]
        public void hide_unless_negative_case()
        {
            new HtmlTag("div").HideUnless(true).HasStyle("display").ShouldBeFalse();
        }

        [Fact]
        public void hide_unless_positive_case()
        {
            new HtmlTag("div").HideUnless(false).Style("display").ShouldBe("none");
        }

        [Fact]
        public void unencoded_turns_off_inner_text_html_encoding()
        {
            var tag = new HtmlTag("div").Text("<img />").UnEncoded();
            tag.Encoded().ShouldBeFalse();
            tag.ToString().ShouldBe("<div><img /></div>");
        }
    }



    
    public class when_converting_an_input_tag_with_a_value_into_a_multiline_editor
    {
        private HtmlTag theTag;
        private string theOriginalValue = "something";

        public when_converting_an_input_tag_with_a_value_into_a_multiline_editor()
        {
            theTag = new HtmlTag("input").Value(theOriginalValue).MultilineMode();
        }

        [Fact]
        public void the_value_attribute_should_removed()
        {
            theTag.HasAttr("value").ShouldBeFalse();
        }

        [Fact]
        public void the_previous_value_should_be_moved_to_the_text_attribute()
        {
            theTag.Text().ShouldBe(theOriginalValue);
        }

        [Fact]
        public void the_tag_name_should_be_changed_to_textarea()
        {
            theTag.TagName().ShouldBe("textarea");
        }
    }

    
    public class when_converting_a_tag_to_multiline_that_does_not_start_with_a_value
    {
        private HtmlTag theTag;

        public when_converting_a_tag_to_multiline_that_does_not_start_with_a_value()
        {
            theTag = new HtmlTag("input").MultilineMode();
        }

        [Fact]
        public void the_tag_name_should_be_changed_to_textarea()
        {
            theTag.TagName().ShouldBe("textarea");
        }
    }
}
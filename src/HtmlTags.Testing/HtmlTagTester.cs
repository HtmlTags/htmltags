using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class HtmlTagTester
    {
        [Test]
        public void do_not_write_closing_tag()
        {
            var tag = new HtmlTag("span").Id("id");
            tag.NoClosingTag();

            tag.ToString().ShouldEqual("<span id=\"id\">");
        }

        [Test]
        public void write_next_if_it_exists()
        {
            var tag = new HtmlTag("span").Text("something");
            tag.Next = new HtmlTag("span").Text("next");

            tag.ToString().ShouldEqual("<span>something</span><span>next</span>");
        }

        [Test]
        public void insert_before()
        {
            var tag = new HtmlTag("div");
            tag.Add("span");
            tag.InsertFirst(new HtmlTag("p"));

            tag.ToString().ShouldEqual("<div><p></p><span></span></div>");
        }

        [Test]
        public void is_input_element()
        {
            var tag = new HtmlTag("span");

            tag.IsInputElement().ShouldBeFalse();
            tag.TagName("input").IsInputElement().ShouldBeTrue();
            tag.TagName("select").IsInputElement().ShouldBeTrue();
            tag.TagName("textarea").IsInputElement().ShouldBeTrue();
        }

        [Test]
        public void is_visible_set_to_false()
        {
            new HtmlTag("div").Visible(false).ToString().ShouldEqual("");
        }

        [Test]
        public void is_visible_set_to_true_by_default()
        {
            var tag = new HtmlTag("div");

            tag.Visible().ShouldBeTrue();
            tag.ToString().ShouldEqual("<div></div>");
        }

        [Test]
        public void prepend()
        {
            HtmlTag tag = new HtmlTag("div").Text("something");
            tag.Prepend("more in front ");
            tag.ToString().ShouldEqual("<div>more in front something</div>");
        }

        [Test]
        public void render_a_single_attribute()
        {
            HtmlTag tag = new HtmlTag("table").Attr("cellPadding", 2);
            tag.ToString().ShouldEqual("<table cellPadding=\"2\"></table>");
        }

        [Test]
        public void render_a_single_class_even_though_it_is_registered_more_than_once()
        {
            HtmlTag tag = new HtmlTag("div").Text("text");
            tag.AddClass("a");

            tag.ToString().ShouldEqual("<div class=\"a\">text</div>");

            tag.AddClass("a");

            tag.ToString().ShouldEqual("<div class=\"a\">text</div>");
        }

        [Test]
        public void can_get_classes_from_tag()
        {
            HtmlTag tag = new HtmlTag("div");

            var classes = new[] {"class1", "class2"};

            tag.AddClasses(classes);

            var tagClasses = tag.GetClasses();

            tagClasses.ShouldHaveCount(2);
            tagClasses.Except(classes).ShouldHaveCount(0);

        }

        [Test]
        public void render_id()
        {
            HtmlTag tag = new HtmlTag("div").Id("theDiv");
            tag.ToString().ShouldEqual("<div id=\"theDiv\"></div>");
        }

        [Test]
        public void render_metadata()
        {
            HtmlTag tag = new HtmlTag("div").Text("text");
            tag.MetaData("a", 1);
            tag.MetaData("b", "b-value");

            tag.ToString().ShouldEqual("<div class=\"{&quot;a&quot;:1,&quot;b&quot;:&quot;b-value&quot;}\">text</div>");

            // now with another class
            tag.AddClass("class1");

            tag.ToString().ShouldEqual("<div class=\"class1 {&quot;a&quot;:1,&quot;b&quot;:&quot;b-value&quot;}\">text</div>");
        }

        [Test]
        public void retrieve_a_previously_set_metadata()
        {
            var tag = new HtmlTag("div");
            tag.MetaData("name", "joe");
            tag.MetaData("name").ShouldEqual("joe");
        }

        [Test]
        public void retrieve_a_non_existing_metadata_should_return_null()
        {
            var tag = new HtmlTag("div");
            tag.MetaData("name").ShouldBeNull();
        }

        [Test]
        public void manipulate_a_previously_set_metadata()
        {
            var tag = new HtmlTag("div");
            tag.MetaData("error", new ListValue{Display = "Original"});
            tag.MetaData<ListValue>("error", val => val.Display = "Changed");
            tag.MetaData("error").As<ListValue>().Display.ShouldEqual("Changed");
        }

        [Test]
        public void attempt_to_manipulate_a_non_existing_metadata_should_be_a_no_op()
        {
            var tag = new HtmlTag("div");
            tag.MetaData<ListValue>("error", val => val.Display = "Changed");
            tag.MetaData("error").ShouldBeNull();
        }


        [Test]
        public void render_multiple_attributes()
        {
            HtmlTag tag = new HtmlTag("table").Attr("cellPadding", "2").Attr("cellSpacing", "3");
            tag.ToString().ShouldEqual("<table cellPadding=\"2\" cellSpacing=\"3\"></table>");
        }

        [Test]
        public void remove_attribute()
        {
            var tag = new HtmlTag("div");
            tag.Attr("foo", "bar");
            tag.HasAttr("foo").ShouldBeTrue();
            
            tag.RemoveAttr("foo");
            tag.HasAttr("foo").ShouldBeFalse();

        }

        [Test]
        public void render_multiple_classes()
        {
            HtmlTag tag = new HtmlTag("div").Text("text");
            tag.AddClass("a");
            tag.AddClass("b");
            tag.AddClass("c");

            tag.ToString().ShouldEqual("<div class=\"a b c\">text</div>");
        }

        [Test]
        public void render_multiple_classes_with_a_single_method_call()
        {
            HtmlTag tag = new HtmlTag("div").Text("text");
            tag.AddClasses("a", "b", "c");

            tag.ToString().ShouldEqual("<div class=\"a b c\">text</div>");
        }

        [Test]
        public void do_not_allow_spaces_in_class_names()
        {
            HtmlTag tag = new HtmlTag("div").Text("text");
            typeof(ArgumentException).ShouldBeThrownBy(() =>
            {
                tag.AddClass("a b c");
            });
        }

        [Test]
        public void do_allow_a_class_that_is_a_json_blob_with_spaces()
        {
            var tag = new HtmlTag("div").AddClass("{a:1, a:2}");
            tag.ToString().ShouldContain("class=\"{a:1, a:2}\"");
        }

        [Test]
        public void render_multiple_levels_of_nesting()
        {
            var tag = new HtmlTag("table");
            tag.Add("tbody/tr/td").Text("some text");

            tag.ToString()
                .ShouldEqual("<table><tbody><tr><td>some text</td></tr></tbody></table>");
        }

        [Test]
        public void render_multiple_levels_of_nesting_2()
        {
            HtmlTag tag = new HtmlTag("html").Modify(x =>
            {
                x.Add("head", head =>
                {
                    head.Add("title").Text("The title");
                    head.Add("style").Text("the style");
                });

                x.Add("body/div").Text("inner text of div");
            });

            tag.ToString().ShouldEqual(
                "<html><head><title>The title</title><style>the style</style></head><body><div>inner text of div</div></body></html>");
        }

        [Test]
        public void render_simple_tag()
        {
            var tag = new HtmlTag("p");
            tag.ToString().ShouldEqual("<p></p>");
        }

        [Test]
        public void render_simple_tag_with_inner_text()
        {
            HtmlTag tag = new HtmlTag("p").Text("some text");
            tag.ToString().ShouldEqual("<p>some text</p>");
        }

        [Test]
        public void render_tag_with_one_child()
        {
            HtmlTag tag = new HtmlTag("div").Child(new HtmlTag("span").Text("something"));
            tag.ToString().ShouldEqual("<div><span>something</span></div>");
        }

        [Test]
        public void replace_a_single_attribute()
        {
            HtmlTag tag = new HtmlTag("table")
                .Attr("cellPadding", 2)
                .Attr("cellPadding", 5);
            tag.ToString().ShouldEqual("<table cellPadding=\"5\"></table>");
        }

        [Test]
        public void the_inner_text_is_html_encoded()
        {
            var tag = new HtmlTag("div");
            tag.Text("<b>Hi</b>");
            tag.ToString().ShouldEqual("<div>&lt;b&gt;Hi&lt;/b&gt;</div>");
        }

        [Test]
        public void the_inner_text_is_html_unencoded()
        {
            var tag = new HtmlTag("div");
            tag.Text("<b>Hi</b>");
            tag.UnEncoded();

            tag.ToString().ShouldEqual("<div><b>Hi</b></div>");
        }

        [Test]
        public void should_respect_subclass_encode_preference()
        {
            var tag = new NonEncodedTag("div");
            tag.Text("<b>Hi</b>");
            tag.ToString().ShouldEqual("<div><b>Hi</b></div>");
        }

        [Test]
        public void write_styles()
        {
            new HtmlTag("div")
                .Style("padding-left", "20px")
                .Style("padding-right", "30px")
                .ToString()
                .ShouldEqual("<div style=\"padding-left:20px;padding-right:30px\"></div>");
        }

        [Test]
        public void write_deep_object_in_metadata()
        {
            new HtmlTag("div").MetaData("listValue", new ListValue
            {
                Display = "a",
                Value = "1"
            }).ToString().ShouldEqual("<div class=\"{&quot;listValue&quot;:{&quot;Display&quot;:&quot;a&quot;,&quot;Value&quot;:&quot;1&quot;}}\"></div>");
        }

        [Test]
        public void wrap_with_copies_the_visibility_from_the_inner_value_positive_case()
        {
            var tag = new HtmlTag("a");
            tag.Visible().ShouldBeTrue();
            tag.WrapWith("span").Visible().ShouldBeTrue();
        }


        [Test]
        public void wrap_with_copies_the_visibility_from_the_inner_value_negative_case()
        {
            var tag = new HtmlTag("a").Visible(false);
            tag.WrapWith("span").Visible().ShouldBeFalse();
        }


        [Test]
        public void wrap_with_copies_the_authorization_from_the_inner_value_positive_case()
        {
            var tag = new HtmlTag("a");
            tag.Authorized().ShouldBeTrue();
            tag.WrapWith("span").Authorized().ShouldBeTrue();
        }


        [Test]
        public void wrap_with_copies_the_authorization_from_the_inner_value_negative_case()
        {
            var tag = new HtmlTag("a").Authorized(false);
            tag.WrapWith("span").Authorized().ShouldBeFalse();
        }

        [Test]
        public void wrap_with_returns_a_new_tag_with_the_original_as_the_first_child()
        {
            var tag = new HtmlTag("a");
            var wrapped = tag.WrapWith("span");

            wrapped.ShouldNotBeTheSameAs(tag);
            wrapped.TagName().ShouldEqual("span");
            wrapped.FirstChild().ShouldBeTheSameAs(tag);
        }

        [Test]
        public void wrap_with_another_tag_returns_the_second_tag_with_the_first_as_the_child()
        {
            var tag = new HtmlTag("a");
            var wrapper = new HtmlTag("span");

            tag.WrapWith(wrapper).ShouldBeTheSameAs(wrapper);
            wrapper.FirstChild().ShouldBeTheSameAs(tag);
        }


        [Test]
        public void is_authorized_by_default()
        {
            new HtmlTag("div").Authorized().ShouldBeTrue();
        }

        [Test]
        public void is_authorized_value_false_makes_tag_hidden_regardless_of_visibility()
        {
            var tag = new HtmlTag("div").Authorized(false);
            tag.ToString().ShouldBeEmpty();

            tag.Visible(true).ToString().ShouldBeEmpty();
            tag.Visible(false).ToString().ShouldBeEmpty();
        }

        [Test]
        public void remove_a_class()
        {
            var tag = new HtmlTag("div").AddClasses("a", "b", "c");
            tag.RemoveClass("b");
            tag.ToString().ShouldEqual("<div class=\"a c\"></div>");
        }

        public class ListValue
        {
            public string Display { get; set; }
            public string Value { get; set; }
        }

        public class NonEncodedTag : HtmlTag
        {
            public NonEncodedTag(string tag) : base(tag)
            {
                EncodeInnerText = false;
            }

            public NonEncodedTag(string tag, Action<HtmlTag> configure) : base(tag, configure)
            {
                EncodeInnerText = false;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Should;
using Xunit;
using System.Text;

namespace HtmlTags.Testing
{
    
    public class core_behavior_tests
    {
        [Fact]
        public void render_a_tag()
        {
            var tag = new HtmlTag("p");
            tag.ToString().ShouldEqual("<p></p>");
        }

        [Fact]
        public void render_a_tag_with_inner_text()
        {
            var tag = new HtmlTag("p").Text("some text");
            tag.ToString().ShouldEqual("<p>some text</p>");
        }

        [Fact]
        public void the_inner_text_is_html_encoded_by_default()
        {
            var tag = new HtmlTag("div");
            tag.Text("<b>Hi</b>");
            tag.ToString().ShouldEqual("<div>&lt;b&gt;Hi&lt;/b&gt;</div>");
        }

        [Fact]
        public void can_opt_out_of_html_encoded_inner_text()
        {
            var tag = new HtmlTag("div");
            tag.Text("<b>Hi</b>");
            tag.Encoded(false);

            tag.ToString().ShouldEqual("<div><b>Hi</b></div>");
        }

        [Fact]
        public void should_respect_subclass_encode_preference()
        {
            var tag = new NonEncodedTag("div");
            tag.Text("<b>Hi</b>");
            tag.ToString().ShouldEqual("<div><b>Hi</b></div>");
        }

        [Fact]
        public void implements_to_html_string_for_aspnet_4_compatibility()
        {
            var tag = new HtmlTag("p");
            tag.ToHtmlString().ShouldEqual("<p></p>");
        }

        [Fact]
        public void pretty_string_is_more_suitable_for_human_viewing()
        {
            var tag = new HtmlTag("div");
            new HtmlTag("p", tag).Id("intro").Text("Once upon a midnight...");
            
            var expected = new StringBuilder();
            expected.AppendLine("<div>");
            expected.AppendLine(@"  <p id=""intro"">Once upon a midnight...</p>");
            expected.Append("</div>");
			
            tag.ToPrettyString().ShouldEqual(expected.ToString());
        }

        [Fact]
        public void has_closing_tag_by_default()
        {
            new HtmlTag("div").HasClosingTag().ShouldBeTrue();
        }

        [Fact]
        public void renders_closing_tag_by_default()
        {
            new HtmlTag("div").ToString().ShouldEqual("<div></div>");
        }

        [Fact]
        public void do_not_write_closing_tag()
        {
            var tag = new HtmlTag("span").Id("id");
            tag.NoClosingTag();

            tag.ToString().ShouldEqual("<span id=\"id\">");
        }

        [Fact]
        public void when_no_closing_tag_then_has_is_false()
        {
            new HtmlTag("div").NoClosingTag().HasClosingTag().ShouldBeFalse();
        }

		[Fact]
		public void when_no_tag_do_not_write_opening_or_closing_tag()
		{
			new HtmlTag("div").NoTag().AppendHtml("Hello")
				.ToString().ShouldEqual("Hello");
		}

		[Fact]
		public void when_no_tag_HasClosingTag_is_false()
		{
			new HtmlTag("div").NoTag().HasClosingTag().ShouldBeFalse();
		}

		[Fact]
		public void when_no_tag_HasTag_is_false()
		{
			new HtmlTag("div").NoTag().HasTag().ShouldBeFalse();
		}

        [Fact]
        public void when_placeholder_do_not_write_opening_or_closing_tag()
        {

            HtmlTag.Placeholder().AppendHtml("Hello")
                .ToString().ShouldEqual("Hello");
        }

        [Fact]
        public void when_placeholder_HasClosingTag_is_false()
        {
            HtmlTag.Placeholder().HasClosingTag().ShouldBeFalse();
        }

        [Fact]
        public void when_placeholder_HasTag_is_false()
        {
            HtmlTag.Placeholder().HasTag().ShouldBeFalse();
        }
        
        [Fact]
        public void tag_with_NoClosingTag_should_be_wrapped_correctly_with_tag_with_closingTag()
        {
            var wrapper = new HtmlTag("div");
            var tag = new HtmlTag("input").NoClosingTag();
            wrapper.Append(tag);
            wrapper.ToString().ShouldEqual("<div><input /></div>");
        }

        [Fact]
        public void initialize_tag_via_constructor()
        {
            var tag = new HtmlTag("div", x =>
            {
                x.Id("me");
                x.Attr("title", "tooltip");
            });

            tag.ToString().ShouldEqual("<div id=\"me\" title=\"tooltip\"></div>");
        }

        [Fact]
        public void recognize_input_elements()
        {
            new HtmlTag("span").IsInputElement().ShouldBeFalse();

            new HtmlTag("input").IsInputElement().ShouldBeTrue();
            new HtmlTag("select").IsInputElement().ShouldBeTrue();
            new HtmlTag("textarea").IsInputElement().ShouldBeTrue();
        }

        //
        // HTML attributes
        //

        [Fact]
        public void render_a_single_attribute()
        {
            var tag = new HtmlTag("table").Attr("cellPadding", 2);
            tag.ToString().ShouldEqual("<table cellPadding=\"2\"></table>");
        }

        [Fact]
        public void render_multiple_attributes()
        {
            var tag = new HtmlTag("table").Attr("cellPadding", "2").Attr("cellSpacing", "3");
            tag.ToString().ShouldEqual("<table cellPadding=\"2\" cellSpacing=\"3\"></table>");
        }

        [Fact]
        public void attributes_are_encoded_by_default()
        {
            const string options = "options: availableMeals, optionsText: 'mealName'";
#if ASPNETCORE
            var expectedAfterEncodingText = options.Replace("'", "&#x27;");
#else
            var expectedAfterEncodingText = options.Replace("'", "&#39;");
#endif

            var tag = new HtmlTag("div").Attr("data-bind", options);
            tag.ToString().ShouldContain(expectedAfterEncodingText);
        }

        [Fact]
        public void attributes_can_be_unencoded_if_needed()
        {
            const string options = "options: availableMeals, optionsText: 'mealName'";
            var tag = new HtmlTag("div").UnencodedAttr("bind", options);
            tag.ToString().ShouldContain(options);
        }

        [Fact]
        public void remove_attribute()
        {
            var tag = new HtmlTag("div");
            tag.Attr("foo", "bar");
            tag.HasAttr("foo").ShouldBeTrue();

            tag.RemoveAttr("foo");
            tag.HasAttr("foo").ShouldBeFalse();
        }

        [Fact]
        public void render_id()
        {
            var tag = new HtmlTag("div").Id("theDiv");
            tag.ToString().ShouldEqual("<div id=\"theDiv\"></div>");
        }

        [Fact]
        public void retrieve_a_set_id()
        {
            var tag = new HtmlTag("div").Id("the-div");
            tag.Id().ShouldEqual("the-div");
        }

        [Fact]
        public void set_the_title_attribute()
        {
            var tag = new HtmlTag("div").Title("My Title");
            tag.ToString().ShouldEqual("<div title=\"My Title\"></div>");
        }

        [Fact]
        public void retrieve_the_title()
        {
            var tag = new HtmlTag("div").Title("My Title");
            tag.Title().ShouldEqual("My Title");
        }

        [Fact]
        public void attr_can_be_used_to_add_css_class()
        {
            var tag = new HtmlTag("a");
            tag.Attr("class", "test-class");
            tag.HasClass("test-class").ShouldBeTrue();
        }

        [Fact]
        public void attr_add_multiple_classes_with_space_separated_classes()
        {
            var tag = new HtmlTag("a");
            tag.AddClass("added-class");
            tag.Attr("class", "test-class1 test-class2");
            tag.HasClass("added-class").ShouldBeTrue();
            tag.HasClass("test-class1").ShouldBeTrue();
            tag.HasClass("test-class2").ShouldBeTrue();
        }

        [Fact]
        public void attr_add_multiple_classes_with_multiple_space_separated_classes()
        {
            var tag = new HtmlTag("a");
            tag.Attr("class", "test-class1     test-class2 test-class3  test-class4  ");
            tag.HasClass("test-class1").ShouldBeTrue();
            tag.HasClass("test-class2").ShouldBeTrue();
            tag.HasClass("test-class3").ShouldBeTrue();
            tag.HasClass("test-class4").ShouldBeTrue();
            tag.HasClass(string.Empty).ShouldBeFalse();
        }

        [Fact]
        public void add_multiple_classes_at_once_with_duplicates()
        {
            var tag = new HtmlTag("div").Text("text");
            tag.AddClass("a b c a c");

            tag.ToString().ShouldEqual("<div class=\"a b c\">text</div>");
        }

        [Fact]
        public void add_multiple_classes_at_once()
        {
            var tag = new HtmlTag("div").Text("text");
            tag.AddClass("a b c");

            tag.ToString().ShouldEqual("<div class=\"a b c\">text</div>");
        }

        [Fact]
        public void add_multiple_classes_at_once_with_multiple_spaces()
        {
            var tag = new HtmlTag("div").Text("text");
            tag.AddClass("a    b  c d  e   ");

            tag.ToString().ShouldEqual("<div class=\"a b c d e\">text</div>");
        }

        [Fact]
        public void replace_a_single_attribute()
        {
            var tag = new HtmlTag("table")
                .Attr("cellPadding", 2)
                .Attr("cellPadding", 5);
            tag.ToString().ShouldEqual("<table cellPadding=\"5\"></table>");
        }

        [Fact]
        public void set_an_attribute_to_null_should_remove_the_attribute()
        {
            var tag = new HtmlTag("div");
            tag.Attr("name", "bill");
            tag.HasAttr("name").ShouldBeTrue();
            tag.Attr("name", null);
            tag.HasAttr("name").ShouldBeFalse();
        }

        [Fact]
        public void set_the_class_attribute_to_null_should_remove_all_classes()
        {
            var tag = new HtmlTag("div");
            tag.AddClass("first");
            tag.AddClass("second");
            tag.Attr("class", null);
            tag.ToString().ShouldEqual("<div></div>");
        }

        [Fact]
        public void set_the_class_attribute_to_empty_string_should_remove_all_classes()
        {
            var tag = new HtmlTag("div");
            tag.AddClass("first");
            tag.AddClass("second");
            tag.Attr("class", string.Empty);
            tag.ToString().ShouldEqual("<div></div>");
        }

        [Fact]
        public void removing_the_class_attribute_should_remove_all_classes()
        {
            var tag = new HtmlTag("div");
            tag.AddClass("first");
            tag.AddClass("second");
            tag.RemoveAttr("class");
            tag.ToString().ShouldEqual("<div></div>");
        }

        [Fact]
        public void should_report_has_class_attribute_if_any_classes_added()
        {
            var tag = new HtmlTag("div");
            tag.HasAttr("class").ShouldBeFalse();
            tag.AddClass("first");
            tag.HasAttr("class").ShouldBeTrue();
        }

        [Fact]
        public void retrieve_a_non_existing_attr_should_return_an_empty_string()
        {
            new HtmlTag("div").Attr("new").ShouldEqual(string.Empty);
        }

        [Fact]
        public void boolean_attr() {
            var tag = new HtmlTag("input").BooleanAttr("required");
            tag.HasAttr("required").ShouldBeTrue();
        }

        [Fact]
        public void render_boolean_attr() {
            var tag = new HtmlTag("input").BooleanAttr("required");
            tag.ToString().ShouldEqual("<input required />");
		}

        [Fact]
        public void set_an_attribute_to_empty_string_should_not_remove_the_attribute() {
            var tag = new HtmlTag("div");
            tag.Attr("name", "bill");
            tag.HasAttr("name").ShouldBeTrue();
            tag.Attr("name", string.Empty);
            tag.HasAttr("name").ShouldBeTrue();
        }

        [Fact]
        public void retrieve_an_empty_attr_should_return_an_empty_string() {
            var tag = new HtmlTag("option");
            tag.Attr("value", string.Empty);
            tag.Attr("value").ShouldEqual(string.Empty);
        }

        [Fact]
        public void render_empty_attr() {
            var tag = new HtmlTag("option").Attr("value", string.Empty);
            tag.ToString().ShouldEqual("<option value=\"\"></option>");
        }

        //
        // CSS classes
        //

        [Fact]
        public void add_a_class()
        {
            var tag = new HtmlTag("div");
            tag.AddClass("a");

            tag.ToString().ShouldEqual("<div class=\"a\"></div>");
        }

        [Fact]
        public void render_multiple_classes()
        {
            var tag = new HtmlTag("div").Text("text");
            tag.AddClass("a");
            tag.AddClass("b");
            tag.AddClass("c");

            tag.ToString().ShouldEqual("<div class=\"a b c\">text</div>");
        }

        [Fact]
        public void render_multiple_classes_with_a_single_method_call()
        {
            var tag = new HtmlTag("div").Text("text");
            tag.AddClasses("a", "b", "c");

            tag.ToString().ShouldEqual("<div class=\"a b c\">text</div>");
        }

        [Fact]
        public void render_multiple_classes_from_a_sequence()
        {
            var tag = new HtmlTag("div").Text("text");
            var sequenceOfClasses = new List<string>{"a", "b", "c"};
            tag.AddClasses(sequenceOfClasses);

            tag.ToString().ShouldEqual("<div class=\"a b c\">text</div>");
        }

        [Fact]
        public void remove_a_class()
        {
            var tag = new HtmlTag("div").AddClasses("a", "b", "c");
            tag.RemoveClass("b");
            tag.ToString().ShouldEqual("<div class=\"a c\"></div>");
        }

        [Fact]
        public void render_a_single_class_even_though_it_is_registered_more_than_once()
        {
            var tag = new HtmlTag("div").Text("text");
            tag.AddClass("a");

            tag.ToString().ShouldEqual("<div class=\"a\">text</div>");

            tag.AddClass("a");

            tag.ToString().ShouldEqual("<div class=\"a\">text</div>");
        }

        [Fact]
        public void can_get_classes_from_tag()
        {
            var tag = new HtmlTag("div");

            var classes = new[] { "class1", "class2" };

            tag.AddClasses(classes);

            var tagClasses = tag.GetClasses();

            tagClasses.ShouldHaveCount(2);
            tagClasses.Except(classes).ShouldHaveCount(0);
        }

        [Fact]
        public void do_not_allow_special_characters_in_class_names()
        {
            CssClassNameValidator.AllowInvalidCssClassNames = false;
            var tag = new HtmlTag("div").Text("text");
            typeof(ArgumentException).ShouldBeThrownBy(() => { tag.AddClass("$test@@"); });
        }

        [Fact]
        public void do_not_allow_start_with_number_in_class_names()
        {
            CssClassNameValidator.AllowInvalidCssClassNames = false;
            var tag = new HtmlTag("div").Text("text");
            typeof(ArgumentException).ShouldBeThrownBy(() => { tag.AddClass("4test"); });
        }

        [Fact]
        public void do_not_allow_first_double_dashes_in_class_names()
        {
            CssClassNameValidator.AllowInvalidCssClassNames = false;
            var tag = new HtmlTag("div").Text("text");
            typeof(ArgumentException).ShouldBeThrownBy(() => { tag.AddClass("--test"); });
        }

        [Fact]
        public void class_name_must_have_at_least_two_chars_if_starts_with_dash()
        {
            CssClassNameValidator.AllowInvalidCssClassNames = false;
            var tag = new HtmlTag("div").Text("text");
            typeof(ArgumentException).ShouldBeThrownBy(() => { tag.AddClass("-"); });
        }

        [Fact]
        public void valid_class_names()
        {
            var tag = new HtmlTag("div").Text("text");
            tag.AddClass("-test");

            tag.ToString().ShouldEqual("<div class=\"-test\">text</div>");

            tag.AddClass("_test");

            tag.ToString().ShouldEqual("<div class=\"-test _test\">text</div>");

            tag.AddClass("TEST_2-test");

            tag.ToString().ShouldEqual("<div class=\"-test _test TEST_2-test\">text</div>");

            tag.AddClass("-just-4-test");

            tag.ToString().ShouldEqual("<div class=\"-test _test TEST_2-test -just-4-test\">text</div>");
        }

        [Fact]
        public void do_allow_a_class_that_is_a_json_blob_with_spaces()
        {
            var tag = new HtmlTag("div").AddClass("{a:1, a:2}");
            tag.ToString().ShouldContain("class=\"{a:1, a:2}\"");

            tag = new HtmlTag("div").AddClass("[1, 2, 3]");
            tag.ToString().ShouldContain("class=\"[1, 2, 3]\"");
        }

        [Fact]
        public void class_name_with_wrong_json()
        {
            var tag = new HtmlTag("div");
            typeof(ArgumentException).ShouldBeThrownBy(() => { tag.AddClass("[1,2,3}"); });
            typeof(ArgumentException).ShouldBeThrownBy(() => { tag.AddClass("{a:1, a:2]"); });
        }

        //
        // inline styles
        //

        [Fact]
        public void write_styles()
        {
            new HtmlTag("div")
                .Style("padding-left", "20px")
                .Style("padding-right", "30px")
                .ToString()
                .ShouldEqual("<div style=\"padding-left:20px;padding-right:30px\"></div>");
        }

        [Fact]
        public void removing_the_style_attribute_should_remove_all_styles()
        {
            var tag = new HtmlTag("div");
            tag.Style("padding-left", "20px").Style("padding-right", "30px");
            tag.HasStyle("padding-left").ShouldBeTrue();
            tag.HasStyle("padding-right").ShouldBeTrue();

            tag.RemoveAttr("style");

            tag.HasStyle("padding-left").ShouldBeFalse();
            tag.HasStyle("padding-right").ShouldBeFalse();
        }

        [Fact]
        public void setting_the_style_attribute_to_empty_string_should_remove_all_styles()
        {
            var tag = new HtmlTag("div");
            tag.Style("padding-left", "20px").Style("padding-right", "30px");
            tag.HasStyle("padding-left").ShouldBeTrue();

            tag.Attr("style", string.Empty);

            tag.HasStyle("padding-left").ShouldBeFalse();
        }

        [Fact]
        public void setting_the_style_attribute_to_null_should_remove_all_styles()
        {
            var tag = new HtmlTag("div");
            tag.Style("padding-left", "20px").Style("padding-right", "30px");
            tag.HasStyle("padding-left").ShouldBeTrue();

            tag.Attr("style", null);

            tag.HasStyle("padding-left").ShouldBeFalse();
        }

        [Fact]
        public void hasattr_should_be_true_for_style_if_any_styles_exist()
        {
            var tag = new HtmlTag("div");
            tag.HasAttr("style").ShouldBeFalse();
            tag.Style("padding-left", "20px");
            tag.HasAttr("style").ShouldBeTrue();
        }
    }

    
    public class children_tests : IDisposable
    {
        public void Dispose()
        {
            BrTag.ComplianceMode = BrTag.ComplianceModes.AspNet;
        }

        [Fact]
        public void add_a_child_tag()
        {
            var tag = new HtmlTag("div").Append(new HtmlTag("span").Text("something"));
            tag.ToString().ShouldEqual("<div><span>something</span></div>");
        }

        [Fact]
        public void create_a_new_tag_as_child_of_existing_tag()
        {
            var existing = new HtmlTag("div");
            var newTag = new HtmlTag("span", existing);
            newTag.Text("something");
            existing.ToString().ShouldEqual("<div><span>something</span></div>");
        }

        [Fact]
        public void append_adds_a_new_child_and_return_the_original()
        {
            var parent = new HtmlTag("div");
            var resultOfAppend = parent.Append("p");
            resultOfAppend.ShouldBeSameAs(parent);
            parent.ToString().ShouldEqual("<div><p></p></div>");
        }

        [Fact]
        public void append_nested_children()
        {
            var parent = new HtmlTag("div");
            parent.Append("p > span");
            parent.ToString().ShouldEqual("<div><p><span></span></p></div>");
        }


        [Fact]
        public void append_all_tags_from_a_tag_source()
        {
            var tagSource = new TagList(new[]{new HtmlTag("br"), new HtmlTag("hr")  });
            var parent = new HtmlTag("div");
            parent.Append(tagSource);
            parent.ToString().ShouldEqual("<div><br /><hr /></div>");
        }

        [Fact]
        public void can_control_br_tag_behavior_even_if_using_regular_html_tag()
        {
            BrTag.ComplianceMode = BrTag.ComplianceModes.Xhtml;
            var tagSource = new TagList(new[] { new HtmlTag("br"), new HtmlTag("hr") });
            var parent = new HtmlTag("div");
            parent.Append(tagSource);
            parent.ToString().ShouldEqual("<div><br /><hr /></div>");
            
        }

        [Fact]
        public void append_all_tags_from_a_sequence()
        {
            var sequence = new[] { new HtmlTag("br"), new HtmlTag("hr") };
            var parent = new HtmlTag("div");
            parent.Append(sequence);
            parent.ToString().ShouldEqual("<div><br /><hr /></div>");
        }

        [Fact]
        public void append_and_modify_the_innermost_child()
        {
            var tag = new HtmlTag("body").Append("div > form > input", child => child.Id("first-name"));
            tag.ToString().ShouldEqual("<body><div><form><input id=\"first-name\" /></form></div></body>");
        }

        [Fact]
        public void insert_a_new_child_tag_as_the_first_child()
        {
            var tag = new HtmlTag("div");
            tag.Append("span");
            tag.InsertFirst(new HtmlTag("p"));

            tag.ToString().ShouldEqual("<div><p></p><span></span></div>");
        }

        [Fact]
        public void add_returns_the_newly_created_child_tag()
        {
            var original = new HtmlTag("div");
            var child = original.Add("span");
            child.ToString().ShouldEqual("<span></span>");
            original.ToString().ShouldEqual("<div><span></span></div>");
        }

        [Fact]
        public void add_and_return_a_child_tag_by_type()
        {
            var original = new HtmlTag("div");
            var child = original.Add<HiddenTag>();
            child.ToString().ShouldEqual("<input type=\"hidden\" />");
            original.ToString().ShouldEqual("<div><input type=\"hidden\" /></div>");
        }

        [Fact]
        public void add_multiple_levels_of_nesting()
        {
            var tag = new HtmlTag("table");
            tag.Add("tbody/tr/td").Text("some text");

            tag.ToString()
                .ShouldEqual("<table><tbody><tr><td>some text</td></tr></tbody></table>");
        }

        [Fact]
        public void nesting_also_supports_jquery_direct_child_syntax()
        {
            var tag = new HtmlTag("table");
            tag.Add("tbody > tr > td").Text("some text");

            tag.ToString()
                .ShouldEqual("<table><tbody><tr><td>some text</td></tr></tbody></table>");
        }

        [Fact]
        public void add_multiple_levels_of_nesting_with_initializer()
        {
            var tag = new HtmlTag("html").Modify(x =>
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

        [Fact]
        public void remove_existing_children_and_add_new_children()
        {
            var tag = new HtmlTag("div").Append("br").Append("p");
            tag.Children.ShouldHaveCount(2);
            tag.ReplaceChildren(new HtmlTag("hr"));
            tag.Children.ShouldHaveCount(1);
            tag.ToString().ShouldEqual("<div><hr /></div>");
        }

        [Fact]
        public void does_not_return_children_or_siblings_when_treated_as_a_tag_source()
        {
            var original = new HtmlTag("div");
            original.Add("span");
            original.After(new HtmlTag("p"));
            var tagSource = (ITagSource) original;
            var allTags = tagSource.AllTags().ToArray();
            allTags.ShouldHaveCount(1);
            allTags[0].ShouldBeSameAs(original);
        }
    }

    
    public class sibling_tests
    {
        [Fact]
        public void set_the_next_sibling_via_next_property()
        {
            var tag = new HtmlTag("span").Text("something");
            tag.Next = new HtmlTag("span").Text("next");

            tag.ToString().ShouldEqual("<span>something</span><span>next</span>");
        }

        [Fact]
        public void set_the_next_sibling_via_next_property_should_overwrite_any_existing_sibling()
        {
            var tag = new HtmlTag("span").Text("something");
            tag.Next = new HtmlTag("span").Text("next");
            tag.Next = new HtmlTag("span").Text("brother");
            tag.ToString().ShouldEqual("<span>something</span><span>brother</span>");
        }


        [Fact]
        public void set_the_next_sibling_via_the_after_method()
        {
            var tag = new HtmlTag("span").Text("something");
            tag.After(new HtmlTag("span").Text("next"));

            tag.ToString().ShouldEqual("<span>something</span><span>next</span>");
        }

        [Fact]
        public void set_the_next_sibling_via_the_after_method_preserves_any_existing_sibling()
        {
            var tag = new HtmlTag("span").Text("something");
            tag.After(new HtmlTag("span").Text("first brother"));
            tag.After(new HtmlTag("span").Text("second brother"));
            tag.ToString().ShouldEqual("<span>something</span><span>second brother</span><span>first brother</span>");
        }

        [Fact]
        public void retrieve_the_next_sibling()
        {
            var tag = new HtmlTag("span").Text("something");
            var nextSibling = new HtmlTag("span").Text("first brother");
            tag.After(nextSibling);

            tag.After().ShouldBeSameAs(nextSibling);
        }

        [Fact]
        public void get_the_next_sibling_via_property_is_equivelent_to_after()
        {
            var tag = new HtmlTag("span").Text("something");
            var nextSibling = new HtmlTag("span").Text("first brother");
            tag.After(nextSibling);

            tag.Next.ShouldBeSameAs(nextSibling);
        }
    }

    
    public class output_control_tests
    {
        [Fact]
        public void hide_renders_the_tag_but_sets_style_to_display_none()
        {
            var tag = new HtmlTag("div").Hide();
            tag.Style("display").ShouldEqual("none");
        }

        [Fact]
        public void render_set_to_true_by_default()
        {
            var tag = new HtmlTag("div");

            tag.Render().ShouldBeTrue();
            tag.ToString().ShouldEqual("<div></div>");
        }

        [Fact]
        public void render_set_to_false_always_returns_an_empty_string()
        {
            new HtmlTag("div").Render(false).ToString().ShouldEqual("");
        }

        [Fact]
        public void tags_are_authorized_by_default()
        {
            new HtmlTag("div").Authorized().ShouldBeTrue();
        }

        [Fact]
        public void unauthorized_tags_will_always_return_an_empty_string_regardless_of_render_setting()
        {
            var tag = new HtmlTag("div").Authorized(false);
            tag.ToString().ShouldBeEmpty();

            tag.Render(true).ToString().ShouldBeEmpty();
            tag.Render(false).ToString().ShouldBeEmpty();
        }

        [Fact]
        public void Empty_can_be_used_as_a_non_rendering_placeholder()
        {
            var tag = new HtmlTag("body").Append(HtmlTag.Empty());
            tag.ToString().Equals("<body></body>");
        }

        [Fact]
        public void rendering_a_NoTag_should_not_render_anything()
        {
            new NoTag().AddClass("important").Text("foo").ToString().ShouldEqual("");
        }
    }

    
    public class wrapping_tags_tests
    {
        [Fact]
        public void wrap_with_returns_a_new_tag_with_the_original_as_the_first_child()
        {
            var tag = new HtmlTag("a");
            var wrapped = tag.WrapWith("span");

            wrapped.ShouldNotBeSameAs(tag);
            wrapped.TagName().ShouldEqual("span");
            wrapped.FirstChild().ShouldBeSameAs(tag);
        }

        [Fact]
        public void wrap_with_another_tag_returns_the_wrapping_tag_with_the_original_as_its_child()
        {
            var tag = new HtmlTag("a");
            var wrapper = new HtmlTag("span");

            tag.WrapWith(wrapper).ShouldBeSameAs(wrapper);
            wrapper.FirstChild().ShouldBeSameAs(tag);
        }

        [Fact]
        public void wrapped_tag_will_be_rendered_if_the_original_tag_was_to_be_rendered()
        {
            var tag = new HtmlTag("a");
            tag.Render().ShouldBeTrue();
            tag.WrapWith("span").Render().ShouldBeTrue();
        }


        [Fact]
        public void wrapped_tag_will_not_be_rendered_if_the_original_tag_was_not_to_be_rendered()
        {
            var tag = new HtmlTag("a").Render(false);
            tag.WrapWith("span").Render().ShouldBeFalse();
        }

        [Fact]
        public void wrapped_tag_will_be_authorized_if_the_original_tag_was_authorized()
        {
            var tag = new HtmlTag("a");
            tag.Authorized().ShouldBeTrue();
            tag.WrapWith("span").Authorized().ShouldBeTrue();
        }

        [Fact]
        public void wrapped_tag_will_not_be_authorized_if_the_original_tag_was_not_authorized()
        {
            var tag = new HtmlTag("a").Authorized(false);
            tag.WrapWith("span").Authorized().ShouldBeFalse();
        }
    }

    
    public class custom_data_attribute_tests
    {
        [Fact]
        public void add_custom_data()
        {
            var tag = new HtmlTag("div");
            tag.Data("integer", 1);
            tag.Data("string", "b-value");
            tag.Data("bool", true);
            tag.Data("setting", new ListValue {Value = "RED", Display = "Red"});
            tag.ToString().ShouldEqual("<div data-integer=\"1\" data-string=\"b-value\" data-bool=\"true\" data-setting=\"{&quot;Display&quot;:&quot;Red&quot;,&quot;Value&quot;:&quot;RED&quot;}\"></div>");
        }

        [Fact]
        public void retrieve_a_previously_set_custom_data_value()
        {
            var tag = new HtmlTag("div");
            tag.Data("age", 42);
            tag.Data("age").ShouldEqual(42);
        }

        [Fact]
        public void retrieve_a_non_existing_custom_data_should_return_null()
        {
            var tag = new HtmlTag("div");
            tag.Data("name").ShouldBeNull();
        }

        [Fact]
        public void set_a_custom_data_to_null_should_remove_the_data_attribute()
        {
            var tag = new HtmlTag("div");
            tag.Data("name", "Luke");
            tag.Data("name", null);
            tag.HasAttr("data-name").ShouldBeFalse();
        }

        [Fact]
        public void set_a_custom_data_to_empty_string_should_store_an_empty_string()
        {
            var tag = new HtmlTag("div");
            tag.Data("name", "");
            tag.HasAttr("data-name").ShouldBeTrue();
            tag.Data("name").ShouldEqual("");
            tag.ToString().ShouldEqual("<div data-name=\"\"></div>");
        }

        [Fact]
        public void manipulate_previously_set_custom_data()
        {
            var tag = new HtmlTag("div");
            tag.Data("error", new ListValue {Display = "Original"});
            tag.Data<ListValue>("error", val => val.Display = "Changed");
            ((ListValue)tag.Data("error")).Display.ShouldEqual("Changed");
        }

        [Fact]
        public void attempt_to_manipulate_non_existing_custom_data_should_be_a_no_op()
        {
            var tag = new HtmlTag("div");
            tag.Data<ListValue>("error", val => val.Display = "Changed");
            tag.Data("error").ShouldBeNull();
        }
    }

    
    public class metadata_tests
    {
        [Fact]
        public void render_metadata()
        {
            var tag = new HtmlTag("div").Text("text");
            tag.MetaData("a", 1);
            tag.MetaData("b", "b-value");

            tag.ToString().ShouldEqual("<div data-__=\"{&quot;a&quot;:1,&quot;b&quot;:&quot;b-value&quot;}\">text</div>");

            // now with another class
            tag.AddClass("class1");

            tag.ToString().ShouldEqual("<div class=\"class1\" data-__=\"{&quot;a&quot;:1,&quot;b&quot;:&quot;b-value&quot;}\">text</div>");
        }

        [Fact]
        public void retrieve_a_previously_set_metadata()
        {
            var tag = new HtmlTag("div");
            tag.MetaData("name", "joe");
            tag.MetaData("name").ShouldEqual("joe");
        }

        [Fact]
        public void retrieve_a_non_existing_metadata_should_return_null()
        {
            var tag = new HtmlTag("div");
            tag.MetaData("name").ShouldBeNull();
        }

        [Fact]
        public void manipulate_a_previously_set_metadata()
        {
            var tag = new HtmlTag("div");
            tag.MetaData("error", new ListValue {Display = "Original"});
            tag.MetaData<ListValue>("error", val => val.Display = "Changed");
            ((ListValue)tag.MetaData("error")).Display.ShouldEqual("Changed");
        }

        [Fact]
        public void attempt_to_manipulate_a_non_existing_metadata_should_be_a_no_op()
        {
            var tag = new HtmlTag("div");
            tag.MetaData<ListValue>("error", val => val.Display = "Changed");
            tag.MetaData("error").ShouldBeNull();
        }

        [Fact]
        public void setting_the_metadata_attribute_to_empty_string_should_remove_all_metadata()
        {
            var tag = new HtmlTag("div");
            tag.MetaData("name", "Luke");
            tag.HasMetaData("name").ShouldBeTrue();

            tag.Attr(HtmlTag.MetadataAttribute, string.Empty);

            tag.HasMetaData("name").ShouldBeFalse();
        }

        [Fact]
        public void setting_the_metadata_attribute_to_null_should_remove_all_metadata()
        {
            var tag = new HtmlTag("div");
            tag.MetaData("name", "Luke");
            tag.HasMetaData("name").ShouldBeTrue();

            tag.Attr(HtmlTag.MetadataAttribute, null);

            tag.HasMetaData("name").ShouldBeFalse();
        }

        [Fact]
        public void hasattr_should_be_true_for_metadata_attribute_if_any_metadata_exists()
        {
            var tag = new HtmlTag("div");
            tag.HasAttr(HtmlTag.MetadataAttribute).ShouldBeFalse();
            tag.MetaData("name", "Luke");
            tag.HasAttr(HtmlTag.MetadataAttribute).ShouldBeTrue();
        }

        [Fact]
        public void write_deep_object_in_metadata()
        {
            new HtmlTag("div").MetaData("listValue", new ListValue
                {
                    Display = "a",
                    Value = "1"
                }).ToString().ShouldEqual("<div data-__=\"{&quot;listValue&quot;:{&quot;Display&quot;:&quot;a&quot;,&quot;Value&quot;:&quot;1&quot;}}\"></div>");
        }

        [Fact]
        public void append_html()
        {
            var tag = new HtmlTag("div");
            tag.AppendHtml("<span>Hello</span>").ShouldBeSameAs(tag);
            tag.AppendHtml(" Hey!");

            tag.ToString().ShouldEqual("<div><span>Hello</span> Hey!</div>");
        }
    }

    
    public class metadataattribute_tests : IDisposable
    {
        public void Dispose()
        {
            HtmlTag.UseMetadataSuffix("__");
        }

        [Fact]
        public void metadataattribute_value_ends_with_a_colon_char_by_default()
        {
            HtmlTag.MetadataAttribute.ShouldEqual("data-__");
        }

        [Fact]
        public void metadataattribute_value_ends_with_the_metadatasuffix_char()
        {
            HtmlTag.UseMetadataSuffix("*");
            HtmlTag.MetadataAttribute.ShouldEqual("data-*");
        }

    }

    internal class ListValue
    {
        public string Display { get; set; }
        public string Value { get; set; }
    }

    internal class NonEncodedTag : HtmlTag
    {
        public NonEncodedTag(string tag)
            : base(tag)
        {
            Encoded(false);
        }
    }
}
using Shouldly;
using Xunit;
using System.Linq;

namespace HtmlTags.Testing
{
    
    public class SelectTagTester
    {
        [Fact]
        public void selected_by_value()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");
            tag.Option("b", "2");
            tag.Option("c", "3");

            tag.SelectByValue("2");

            tag.Children[0].HasAttr("selected").ShouldBeFalse();
            tag.Children[1].Attr("selected").ShouldBe("selected");
            tag.Children[2].HasAttr("selected").ShouldBeFalse();
        }

        [Fact]
        public void selected_by_string_value()
        {
            var tag = new SelectTag();
            tag.Option("a", 1);
            tag.Option("b", 2);
            tag.Option("c", 3);

            tag.SelectByValue(2);

            tag.Children[0].HasAttr("selected").ShouldBeFalse();
            tag.Children[1].Attr("selected").ShouldBe("selected");
            tag.Children[2].HasAttr("selected").ShouldBeFalse();
        }

        [Fact]
        public void can_initialize_options_in_constructor()
        {
            var select = new SelectTag(tag =>
            {
                tag.Option("a", "1");
                tag.Option("b", "2");
                tag.Option("c", "3");
            });
            select.Children.Select(t => t.Text()).ShouldHaveTheSameElementsAs(new[]{"a", "b", "c"});
        }


        [Fact]
        public void should_remove_previous_selected_value()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");
            tag.Option("b", "2");
            tag.Option("c", "3");

            tag.SelectByValue("2");
            tag.SelectByValue("1");

            tag.Children[0].Attr("selected").ShouldBe("selected");
            tag.Children[1].HasAttr("selected").ShouldBeFalse();
            tag.Children[2].HasAttr("selected").ShouldBeFalse();
        }

        [Fact]
        public void should_not_remove_previous_selected_value_if_new_value_is_bogus()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");
            tag.Option("b", "2");

            tag.SelectByValue("1");
            tag.SelectByValue("abcd");

            tag.Children[0].Attr("selected").ShouldBe("selected");
            tag.Children[1].HasAttr("selected").ShouldBeFalse();
        }

        [Fact]
        public void should_add_default_option_to_top_of_list()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");
            tag.Option("b", "2");

            tag.DefaultOption("bar");

            tag.Children[0].Text().ShouldBe("bar");
        }

        [Fact]
        public void should_make_the_default_option_selected()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");
            tag.Option("b", "2");

            tag.DefaultOption("bar");

            tag.Children[0].Attr("selected").ShouldBe("selected");
            tag.Children[1].HasAttr("selected").ShouldBeFalse();
            tag.Children[2].HasAttr("selected").ShouldBeFalse();
        }

        [Fact]
        public void FirstOption_should_prepend_new_option_at_top()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");

            tag.TopOption("_", "0");

            tag.Children[0].Text().ShouldBe("_");
            tag.Children[1].Text().ShouldBe("a");
        }
    }
}
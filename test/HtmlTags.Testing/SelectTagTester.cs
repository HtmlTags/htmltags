using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class SelectTagTester
    {
        [Test]
        public void selected_by_value()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");
            tag.Option("b", "2");
            tag.Option("c", "3");

            tag.SelectByValue("2");

            tag.Children[0].HasAttr("selected").ShouldBeFalse();
            tag.Children[1].Attr("selected").ShouldEqual("selected");
            tag.Children[2].HasAttr("selected").ShouldBeFalse();
        }

        [Test]
        public void selected_by_string_value()
        {
            var tag = new SelectTag();
            tag.Option("a", 1);
            tag.Option("b", 2);
            tag.Option("c", 3);

            tag.SelectByValue(2);

            tag.Children[0].HasAttr("selected").ShouldBeFalse();
            tag.Children[1].Attr("selected").ShouldEqual("selected");
            tag.Children[2].HasAttr("selected").ShouldBeFalse();
        }

        [Test]
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


        [Test]
        public void should_remove_previous_selected_value()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");
            tag.Option("b", "2");
            tag.Option("c", "3");

            tag.SelectByValue("2");
            tag.SelectByValue("1");

            tag.Children[0].Attr("selected").ShouldEqual("selected");
            tag.Children[1].HasAttr("selected").ShouldBeFalse();
            tag.Children[2].HasAttr("selected").ShouldBeFalse();
        }

        [Test]
        public void should_not_remove_previous_selected_value_if_new_value_is_bogus()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");
            tag.Option("b", "2");

            tag.SelectByValue("1");
            tag.SelectByValue("abcd");

            tag.Children[0].Attr("selected").ShouldEqual("selected");
            tag.Children[1].HasAttr("selected").ShouldBeFalse();
        }

        [Test]
        public void should_add_default_option_to_top_of_list()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");
            tag.Option("b", "2");

            tag.DefaultOption("bar");

            tag.Children[0].Text().ShouldEqual("bar");
        }

        [Test]
        public void should_make_the_default_option_selected()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");
            tag.Option("b", "2");

            tag.DefaultOption("bar");

            tag.Children[0].Attr("selected").ShouldEqual("selected");
            tag.Children[1].HasAttr("selected").ShouldBeFalse();
            tag.Children[2].HasAttr("selected").ShouldBeFalse();
        }

        [Test]
        public void FirstOption_should_prepend_new_option_at_top()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");

            tag.TopOption("_", "0");

            tag.Children[0].Text().ShouldEqual("_");
            tag.Children[1].Text().ShouldEqual("a");
        }
    }
}
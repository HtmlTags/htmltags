using Should;
using Xunit;

namespace HtmlTags.Testing
{
    
    public class CheckboxTagTester
    {
        [Fact]
        public void basic_construction()
        {
            var tag = new CheckboxTag(true);
            tag.TagName().ShouldEqual("input");
            tag.Attr("type").ShouldEqual("checkbox");
        }

        [Fact]
        public void create_checkbox_that_is_checked()
        {
            var tag = new CheckboxTag(true);
            tag.Attr("checked").ShouldEqual("true");
        }

        [Fact]
        public void create_checkbox_that_is_not_checked()
        {
            var tag = new CheckboxTag(false);
            tag.HasAttr("checked").ShouldBeFalse();
        }
    }
}
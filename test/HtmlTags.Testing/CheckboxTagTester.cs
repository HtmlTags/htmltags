using FubuTestingSupport;
using NUnit.Framework;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class CheckboxTagTester
    {
        [Test]
        public void basic_construction()
        {
            var tag = new CheckboxTag(true);
            tag.TagName().ShouldEqual("input");
            tag.Attr("type").ShouldEqual("checkbox");
        }

        [Test]
        public void create_checkbox_that_is_checked()
        {
            var tag = new CheckboxTag(true);
            tag.Attr("checked").ShouldEqual("true");
        }

        [Test]
        public void create_checkbox_that_is_not_checked()
        {
            var tag = new CheckboxTag(false);
            tag.HasAttr("checked").ShouldBeFalse();
        }
    }
}
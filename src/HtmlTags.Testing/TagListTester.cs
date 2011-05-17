using NUnit.Framework;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class TagListTester
    {
        [Test]
        public void can_generate_output_for_a_collection_of_tags()
        {
            var tags = new[]{ new HtmlTag("div"), new HtmlTag("p"), new HtmlTag("div")};
            var list = new TagList(tags);
            list.ToString().ShouldEqual("<div></div>\n<p></p>\n<div></div>");
        }

        [Test]
        public void can_be_used_as_a_tag_source()
        {
            var tags = new[] { new HtmlTag("div"), new HtmlTag("p"), new HtmlTag("div") };
            var tagSource = (ITagSource)new TagList(tags);
            tagSource.AllTags().ShouldHaveTheSameElementsAs(tags);
        }
    }
}
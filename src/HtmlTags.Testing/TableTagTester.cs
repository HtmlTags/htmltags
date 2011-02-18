using NUnit.Framework;
using System.Linq;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class TableTagTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void add_caption_always_puts_the_caption_at_top()
        {
            var tag = new TableTag();
            tag.CaptionText("some caption");

            tag.Children[0].Text().ShouldEqual("some caption");
            tag.Children[0].TagName().ShouldEqual("caption");


            tag.CaptionText().ShouldEqual("some caption");

            tag.CaptionText("other caption");
            tag.CaptionText().ShouldEqual("other caption");

            tag.Children.Count(x => x.TagName() == "caption").ShouldEqual(1);
        }
    }
}
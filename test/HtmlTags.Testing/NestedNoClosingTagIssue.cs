using FubuTestingSupport;
using NUnit.Framework;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class NestedNoClosingTagIssue
    {
        [Test]
        public void X()
        {
            var wrapper = new HtmlTag("div");

            var tag = new HtmlTag("input").NoClosingTag();

            wrapper.Append(tag);

            wrapper.ToString()
                .ShouldEqual("<div><input /></div>");
            // actually renders "<div><input />")
        }
    }
}
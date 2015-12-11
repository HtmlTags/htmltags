using Should;
using Xunit;

namespace HtmlTags.Testing
{
    
    public class NestedNoClosingTagIssue
    {
        [Fact]
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
using Shouldly;
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
                .ShouldBe("<div><input></div>");
            // actually renders "<div><input />")
        }
    }
}
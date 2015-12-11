using Xunit;
using Should;

namespace HtmlTags.Testing.Conventions
{
    
    public class Fake
    {
        [Fact]
        public void good()
        {
            true.ShouldBeTrue();
        }

    }
}
using Xunit;
using Shouldly;

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
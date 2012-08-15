using NUnit.Framework;
using FubuTestingSupport;

namespace HtmlTags.Testing.Conventions
{
    [TestFixture]
    public class Fake
    {
        [Test]
        public void good()
        {
            true.ShouldBeTrue();
        }

    }
}
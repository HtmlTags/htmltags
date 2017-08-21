using HtmlTags.Conventions;
using Xunit;

namespace HtmlTags.Testing
{
    public class CachingTests
    {
        [Fact]
        public void Test()
        {
            var library = new HtmlConventionLibrary();
            new DefaultHtmlConventions().Apply(library);
            var generator = ElementGenerator<Foo>.For(library);
            var tag = generator.InputFor(m => m.Value);
            tag = generator.InputFor(m => m.Value);
        }

        public class Foo
        {
            public string Value { get; set; }
        }
    }
}
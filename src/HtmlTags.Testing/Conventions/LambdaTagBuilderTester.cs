using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;

namespace HtmlTags.Testing.Conventions
{
    [TestFixture]
    public class LambdaTagBuilderTester
    {
        [Test]
        public void matches_delegates()
        {
            var builder = new LambdaTagBuilder<FakeSubject>(x => x.Level > 10, x => new HtmlTag("div"));

            builder.Matches(new FakeSubject{Level = 5}).ShouldBeFalse();
            builder.Matches(new FakeSubject{Level = 11}).ShouldBeTrue();
        }

        [Test]
        public void build_delegates()
        {
            var builder = new LambdaTagBuilder<FakeSubject>(x => x.Level > 10, x => new HtmlTag("div").Text(x.Name));

            builder.Build(new FakeSubject{
                Name = "Max"
            })
                .ToString()
                .ShouldEqual("<div>Max</div>");
        }
    }
}
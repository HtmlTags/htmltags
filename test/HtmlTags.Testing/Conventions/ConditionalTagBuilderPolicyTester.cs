using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;

namespace HtmlTags.Testing.Conventions
{
    [TestFixture]
    public class ConditionalTagBuilderPolicyTester
    {
        [Test]
        public void matches_delegates()
        {
            var builder = new ConditionalTagBuilderPolicy(x => ((FakeSubject)x).Level > 10, x => new HtmlTag("div"));

            builder.Matches(new FakeSubject{Level = 5}).ShouldBeFalse();
            builder.Matches(new FakeSubject{Level = 11}).ShouldBeTrue();
        }

        [Test]
        public void build_delegates()
        {
            var builder = new ConditionalTagBuilderPolicy(x => ((FakeSubject)x).Level > 10, x => new HtmlTag("div").Text(((FakeSubject)x).Name));

            var subject = new FakeSubject
            {
                Name = "Max"
            };
            builder.BuilderFor(subject).Build(subject)
                .ToString()
                .ShouldEqual("<div>Max</div>");
        }
    }
}
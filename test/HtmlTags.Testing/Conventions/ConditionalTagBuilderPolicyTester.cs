using Shouldly;
using HtmlTags.Conventions;
using Xunit;

namespace HtmlTags.Testing.Conventions
{
    
    public class ConditionalTagBuilderPolicyTester
    {
        [Fact]
        public void matches_delegates()
        {
            var builder = new ConditionalTagBuilderPolicy(x => ((FakeSubject)x).Level > 10, x => new HtmlTag("div"));

            builder.Matches(new FakeSubject{Level = 5}).ShouldBeFalse();
            builder.Matches(new FakeSubject{Level = 11}).ShouldBeTrue();
        }

        [Fact]
        public void build_delegates()
        {
            var builder = new ConditionalTagBuilderPolicy(x => ((FakeSubject)x).Level > 10, x => new HtmlTag("div").Text(((FakeSubject)x).Name));

            var subject = new FakeSubject
            {
                Name = "Max"
            };
            builder.BuilderFor(subject).Build(subject)
                .ToString()
                .ShouldBe("<div>Max</div>");
        }
    }
}
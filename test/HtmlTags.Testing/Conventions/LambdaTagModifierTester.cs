using Should;
using HtmlTags.Conventions;
using Xunit;

namespace HtmlTags.Testing.Conventions
{
    
    public class LambdaTagModifierTester
    {
        [Fact]
        public void matches_delegates()
        {
            var modifier = new LambdaTagModifier(x => ((FakeSubject)x).Level > 10, x => { });

            modifier.Matches(new FakeSubject { Level = 5 }).ShouldBeFalse();
            modifier.Matches(new FakeSubject { Level = 11 }).ShouldBeTrue();
        }

        [Fact]
        public void modify_delegates()
        {
            var builder = new LambdaTagModifier(x => ((FakeSubject)x).Level > 10, x => x.CurrentTag.AddClass("foo"));

            var subject = new FakeSubject
                              {
                                  Name = "Max"
                              };
            subject.ReplaceTag(new HtmlTag("div"));

            builder.Modify(subject);

            subject.CurrentTag.HasClass("foo").ShouldBeTrue();
        }
    }
}
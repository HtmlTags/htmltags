using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;

namespace HtmlTags.Testing.Conventions
{
    [TestFixture]
    public class LambdaTagModifierTester
    {
        [Test]
        public void matches_delegates()
        {
            var modifier = new LambdaTagModifier<FakeSubject>(x => x.Level > 10, x => { });

            modifier.Matches(new FakeSubject { Level = 5 }).ShouldBeFalse();
            modifier.Matches(new FakeSubject { Level = 11 }).ShouldBeTrue();
        }

        [Test]
        public void modify_delegates()
        {
            var builder = new LambdaTagModifier<FakeSubject>(x => x.Level > 10, x => x.CurrentTag.AddClass("foo"));

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
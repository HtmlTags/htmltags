using HtmlTags.Conventions;
using Xunit;
using Shouldly;

namespace HtmlTags.Testing.Conventions
{

    /*
     * Yes, these are trivial tests, but the framework doesn't work unless the Equals/GetHashCode stuff
     * is predictable
     */

    
    public class TagSubjectTester
    {
        [Fact]
        public void equals()
        {
            var subject1 = new FakeSubject{
                Name = "Jeremy",
                Level = 20
            };

            var subject2 = new FakeSubject{
                Name = "Different",
                Level = int.MaxValue
            };

            subject1.ShouldNotBe(subject2);

            new TagSubject("a", subject1).ShouldBe(new TagSubject("a", subject1));
            new TagSubject("a", subject2).ShouldBe(new TagSubject("a", subject2));
            new TagSubject("a", subject1).ShouldNotBe(new TagSubject("a", subject2));
            new TagSubject("a", subject2).ShouldNotBe(new TagSubject("b", subject2));
        }

        [Fact]
        public void get_hashcode()
        {
            var subject1 = new FakeSubject
            {
                Name = "Jeremy",
                Level = 20
            };

            var subject2 = new FakeSubject
            {
                Name = "Different",
                Level = int.MaxValue
            };

            subject1.ShouldNotBe(subject2);

            new TagSubject("a", subject1).GetHashCode().ShouldBe(new TagSubject("a", subject1).GetHashCode());
            new TagSubject("a", subject2).GetHashCode().ShouldBe(new TagSubject("a", subject2).GetHashCode());
            new TagSubject("a", subject1).GetHashCode().ShouldNotBe(new TagSubject("a", subject2).GetHashCode());
            new TagSubject("a", subject2).GetHashCode().ShouldNotBe(new TagSubject("b", subject2).GetHashCode());
        }
    }
}
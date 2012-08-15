using HtmlTags.Conventions;
using NUnit.Framework;
using FubuTestingSupport;

namespace HtmlTags.Testing.Conventions
{

    /*
     * Yes, these are trivial tests, but the framework doesn't work unless the Equals/GetHashCode stuff
     * is predictable
     */

    [TestFixture]
    public class TagSubjectTester
    {
        [Test]
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

            subject1.ShouldNotEqual(subject2);

            new TagSubject<FakeSubject>("a", subject1).ShouldEqual(new TagSubject<FakeSubject>("a", subject1));
            new TagSubject<FakeSubject>("a", subject2).ShouldEqual(new TagSubject<FakeSubject>("a", subject2));
            new TagSubject<FakeSubject>("a", subject1).ShouldNotEqual(new TagSubject<FakeSubject>("a", subject2));
            new TagSubject<FakeSubject>("a", subject2).ShouldNotEqual(new TagSubject<FakeSubject>("b", subject2));
        }

        [Test]
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

            subject1.ShouldNotEqual(subject2);

            new TagSubject<FakeSubject>("a", subject1).GetHashCode().ShouldEqual(new TagSubject<FakeSubject>("a", subject1).GetHashCode());
            new TagSubject<FakeSubject>("a", subject2).GetHashCode().ShouldEqual(new TagSubject<FakeSubject>("a", subject2).GetHashCode());
            new TagSubject<FakeSubject>("a", subject1).GetHashCode().ShouldNotEqual(new TagSubject<FakeSubject>("a", subject2).GetHashCode());
            new TagSubject<FakeSubject>("a", subject2).GetHashCode().ShouldNotEqual(new TagSubject<FakeSubject>("b", subject2).GetHashCode());
        }
    }
}
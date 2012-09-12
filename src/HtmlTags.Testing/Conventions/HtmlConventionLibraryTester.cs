using System;
using HtmlTags.Conventions;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace HtmlTags.Testing.Conventions
{
    [TestFixture]
    public class HtmlConventionLibraryTester
    {
        [Test]
        public void for_returns_the_same_result()
        {
            var library = new HtmlConventionLibrary();
            var lib1 = library.For<FakeSubject>();
            var lib2 = library.For<FakeSubject>();

            lib1.ShouldBeTheSameAs(lib2);


            library.For<SecondSubject>().ShouldBeTheSameAs(library.For<SecondSubject>());
        }

        [Test]
        public void importing_test()
        {
            var b1 = MockRepository.GenerateMock<ITagBuilder<FakeSubject>>();
            var b2 = MockRepository.GenerateMock<ITagBuilder<FakeSubject>>();
            var b3 = MockRepository.GenerateMock<ITagBuilder<FakeSubject>>();
            var b4 = MockRepository.GenerateMock<ITagBuilder<SecondSubject>>();
            var b5 = MockRepository.GenerateMock<ITagBuilder<SecondSubject>>();
            var b6 = MockRepository.GenerateMock<ITagBuilder<SecondSubject>>();


            var lib1 = new HtmlConventionLibrary();
            lib1.For<FakeSubject>().Add(b1);
            lib1.For<FakeSubject>().Add(b2);

            var lib2 = new HtmlConventionLibrary();
            lib2.For<FakeSubject>().Add(b3);
            lib2.For<SecondSubject>().Add(b4);
            lib2.For<SecondSubject>().Add(b5);
            lib2.For<SecondSubject>().Add(b6);

            lib1.Import(lib2);

            lib1.For<FakeSubject>().Default.Defaults.Builders.ShouldHaveTheSameElementsAs(b1, b2, b3);
            lib1.For<SecondSubject>().Default.Defaults.Builders.ShouldHaveTheSameElementsAs(b4, b5, b6);
        }
    }

    public class SecondSubject : TagRequest
    {
        public override object ToToken()
        {
            throw new NotImplementedException();
        }
    }
}
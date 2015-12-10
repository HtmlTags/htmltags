using System;
using HtmlTags.Conventions;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace HtmlTags.Testing.Conventions
{
    using Reflection;

    [TestFixture]
    public class HtmlConventionLibraryTester
    {
        [Test]
        public void importing_test()
        {
            var b1 = MockRepository.GenerateMock<ITagBuilderPolicy>();
            var b2 = MockRepository.GenerateMock<ITagBuilderPolicy>();
            var b3 = MockRepository.GenerateMock<ITagBuilderPolicy>();
            var b4 = MockRepository.GenerateMock<ITagBuilderPolicy>();
            var b5 = MockRepository.GenerateMock<ITagBuilderPolicy>();
            var b6 = MockRepository.GenerateMock<ITagBuilderPolicy>();


            var lib1 = new HtmlConventionLibrary();
            lib1.TagLibrary.Add(b1);
            lib1.TagLibrary.Add(b2);

            var lib2 = new HtmlConventionLibrary();
            lib2.TagLibrary.Add(b3);
            lib2.TagLibrary.Add(b4);
            lib2.TagLibrary.Add(b5);
            lib2.TagLibrary.Add(b6);

            lib1.Import(lib2);

            lib1.TagLibrary.Default.Defaults.Policies.ShouldHaveTheSameElementsAs(b1, b2, b3, b4, b5, b6);
        }

        [Test]
        public void importing_services()
        {
            var builder1 = new ServiceBuilder();
            var builder2 = new ServiceBuilder();
        
            builder1.Add(() => "blue");
            builder2.Add(() => "green");

            builder2.Add(() => 1);

            builder2.FillInto(builder1);

            builder1.Build<string>().ShouldEqual("blue"); // not replaced
            builder1.Build<int>().ShouldEqual(1); // was filled in
        }

        [Test]
        public void services_are_imported()
        {
            var library1 = new HtmlConventionLibrary();
            library1.RegisterService<IFoo, LittleFoo>();

            var library2 = new HtmlConventionLibrary();
            library2.RegisterService<IFoo, BigFoo>("different");

            library1.Import(library2);

            library1.Get<IFoo>().ShouldBeOfType<LittleFoo>();
            library1.Get<IFoo>("different").ShouldBeOfType<BigFoo>();
        }

        [Test]
        public void services_are_not_overwritten_while_importing()
        {
            var library1 = new HtmlConventionLibrary();
            library1.RegisterService<IFoo, LittleFoo>();

            var library2 = new HtmlConventionLibrary();
            library2.RegisterService<IFoo, BigFoo>();

            library1.Import(library2);

            library1.Get<IFoo>().ShouldBeOfType<LittleFoo>();
        }


        [Test]
        public void get_with_no_prior_definition_will_happily_blow_up()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                new HtmlConventionLibrary().Get<IFoo>();
            }).Message.ShouldContain("No service implementation is registered for type " + typeof(IFoo).FullName);
        }

        [Test]
        public void simple_registration_of_service_by_type()
        {
            var library = new HtmlConventionLibrary();
            library.RegisterService<IFoo, Foo>();

            library.Get<IFoo>().ShouldBeOfType<Foo>();
        }

        [Test]
        public void registration_of_service_by_func()
        {
            var library = new HtmlConventionLibrary();
            library.RegisterService<IFoo>(() => new ColoredFoo{Color = "Red"});

            library.Get<IFoo>().ShouldBeOfType<ColoredFoo>()
                .Color.ShouldEqual("Red");
        }

        [Test]
        public void if_not_explicitly_specified_assume_the_profile_is_default()
        {
            var library = new HtmlConventionLibrary();
            library.RegisterService<IFoo, Foo>(TagConstants.Default);

            library.Get<IFoo>().ShouldBeOfType<Foo>();
        }

        [Test]
        public void register_and_build_by_profile()
        {
            var library = new HtmlConventionLibrary();
            library.RegisterService<IFoo, Foo>(TagConstants.Default);

            library.RegisterService<IFoo, DifferentFoo>("Profile1");

            library.Get<IFoo>("Profile1").ShouldBeOfType<DifferentFoo>();
        }

        [Test]
        public void fetching_by_profile_should_fall_back_to_default_if_not_specific_implementation_is_used()
        {
            var library = new HtmlConventionLibrary();
            library.RegisterService<IFoo, Foo>(TagConstants.Default);

            library.Get<IFoo>("Profile1").ShouldBeOfType<Foo>();
        }
    }

    public interface IFoo{}
    public class Foo : IFoo{}
    public class DifferentFoo : IFoo{}
    public class ColoredFoo : IFoo
    {
        public string Color { get; set; }
    }

    public class BigFoo : IFoo { }
    public class LittleFoo : IFoo { }

    public class SecondSubject : ElementRequest
    {
        public SecondSubject() : base(SingleProperty.Build<SecondSubject>(m => m.ElementId))
        {
            
        }
    }
}
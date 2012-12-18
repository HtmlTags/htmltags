using HtmlTags.Conventions;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace HtmlTags.Testing.Conventions
{
    [TestFixture]
    public class TagLibraryTester
    {
        private TagLibrary<FakeSubject> theLibrary;

        [SetUp]
        public void SetUp()
        {
            theLibrary = new TagLibrary<FakeSubject>();
        }

        private HtmlTag build(FakeSubject subject, string category = null,string profile = null)
        {
            var plan = theLibrary.PlanFor(subject, profile:profile, category:category);
            return plan.Build(subject);
        }

        [Test]
        public void builds_default_if_no_category_or_profile_is_specified()
        {
            theLibrary.Always.Build(x => new HtmlTag("div").Text(x.Name));
            theLibrary.Category("a").Always.Build(x => new HtmlTag("a").Text(x.Name));
            theLibrary.Category("b").Always.Build(x => new HtmlTag("b").Text(x.Name));
        
            theLibrary.ForProfile("profile1").Always.Build(x => new HtmlTag("p").Text(x.Name));
            theLibrary.Category("a").ForProfile("a-1").Always.Modify(x => x.CurrentTag.AddClass("a-1"));

            var subject = new FakeSubject { Name = "Lindsey" };

            build(subject).ToString().ShouldEqual("<div>Lindsey</div>");
            build(subject, category:"a").ToString().ShouldEqual("<a>Lindsey</a>");
            build(subject, category:"a", profile:"a-1").ToString().ShouldEqual("<a class=\"a-1\">Lindsey</a>");
            build(subject, category:"b").ToString().ShouldEqual("<b>Lindsey</b>");
            build(subject, profile:"profile1").ToString().ShouldEqual("<p>Lindsey</p>");
        }

        [Test]
        public void accept_visitor()
        {
            var categoryA = theLibrary.Category("a");
            var categoryB = theLibrary.Category("b");

            var a1 = categoryA.Profile("a1");
            var a2 = categoryA.Profile("a2");

            var b1 = categoryB.Profile("b1");
            var b2 = categoryB.Profile("b2");

            var visitor = MockRepository.GenerateMock<ITagLibraryVisitor<FakeSubject>>();

            theLibrary.AcceptVisitor(visitor);

            visitor.AssertWasCalled(x => x.Category("a", categoryA));
            visitor.AssertWasCalled(x => x.BuilderSet("a1", a1));
            visitor.AssertWasCalled(x => x.BuilderSet("a2", a2));

            visitor.AssertWasCalled(x => x.Category("b", categoryB));
            visitor.AssertWasCalled(x => x.BuilderSet("b1", b1));
            visitor.AssertWasCalled(x => x.BuilderSet("b2", b2));
        }

        [Test]
        public void accept_convention_visitor()
        {
            var topVisitor = MockRepository.GenerateMock<IHtmlConventionVisitor>();
            var childVisitor = MockRepository.GenerateMock<ITagLibraryVisitor<FakeSubject>>();

            var library = MockRepository.GenerateMock<TagLibrary<FakeSubject>>();

            library.Expect(x => x.AcceptVisitor(childVisitor));

            topVisitor.Stub(x => x.VisitorFor<FakeSubject>()).Return(childVisitor);

            library.AcceptVisitor(topVisitor);

            library.AssertWasCalled(x => x.AcceptVisitor(childVisitor));
        }
        
    }

    [TestFixture]
    public class TagLibrary_Import_Tester
    {
        private ITagBuilderPolicy<FakeSubject> b1;
        private ITagBuilderPolicy<FakeSubject> b2;
        private ITagBuilderPolicy<FakeSubject> b3;
        private ITagBuilderPolicy<FakeSubject> b4;
        private ITagBuilderPolicy<FakeSubject> b5;
        private ITagBuilderPolicy<FakeSubject> b6;
        private ITagBuilderPolicy<FakeSubject> b7;
        private ITagModifier<FakeSubject> m1;
        private ITagModifier<FakeSubject> m2;
        private ITagModifier<FakeSubject> m3;
        private ITagModifier<FakeSubject> m4;
        private TagLibrary<FakeSubject> library1;

        [SetUp]
        public void SetUp()
        {
            b1 = MockRepository.GenerateMock<ITagBuilderPolicy<FakeSubject>>();
            b2 = MockRepository.GenerateMock<ITagBuilderPolicy<FakeSubject>>();
            b3 = MockRepository.GenerateMock<ITagBuilderPolicy<FakeSubject>>();
            b4 = MockRepository.GenerateMock<ITagBuilderPolicy<FakeSubject>>();
            b5 = MockRepository.GenerateMock<ITagBuilderPolicy<FakeSubject>>();
            b6 = MockRepository.GenerateMock<ITagBuilderPolicy<FakeSubject>>();
            b7 = MockRepository.GenerateMock<ITagBuilderPolicy<FakeSubject>>();

            m1 = MockRepository.GenerateMock<ITagModifier<FakeSubject>>();
            m2 = MockRepository.GenerateMock<ITagModifier<FakeSubject>>();
            m3 = MockRepository.GenerateMock<ITagModifier<FakeSubject>>();
            m4 = MockRepository.GenerateMock<ITagModifier<FakeSubject>>();

            library1 = new TagLibrary<FakeSubject>();


            library1.Add(b1);
            library1.Add(m1);

            library1.Default.Profile("A").Add(b2);
            library1.Default.Profile("A").Add(m2);

            library1.Category("Cat1").Add(b3);
            library1.Category("Cat1").Add(m3);

            library1.Category("Cat1").Profile("A").Add(b4);


            var library2 = new TagLibrary<FakeSubject>();
            library2.Add(b5);
            library2.Add(m4);

            library2.Category("Cat2").Add(b6);
            library2.Category("Cat1").Add(b7);

            library1.Import(library2);
        }

        [Test]
        public void imports_defaults()
        {
            library1.Default.Defaults.Policies.ShouldHaveTheSameElementsAs(b1, b5);
        }

        [Test]
        public void imports_category_that_both_libraries_have()
        {
            library1.Category("Cat1").Defaults.Policies.ShouldHaveTheSameElementsAs(b3, b7);
        }

        [Test]
        public void imports_category_from_the_second_library_not_originally_in_the_first()
        {
            library1.Category("Cat2").Defaults.Policies.ShouldHaveTheSameElementsAs(b6);
        }


    }
}
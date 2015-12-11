using HtmlTags.Conventions;
using Xunit;
using Should;
using Moq;

namespace HtmlTags.Testing.Conventions
{
    
    public class TagLibraryTester
    {
        private TagLibrary theLibrary;

        public TagLibraryTester()
        {
            theLibrary = new TagLibrary();
        }

        private HtmlTag build(FakeSubject subject, string category = null,string profile = null)
        {
            var plan = theLibrary.PlanFor(subject, profile:profile, category:category);
            return plan.Build(subject);
        }

        [Fact]
        public void builds_default_if_no_category_or_profile_is_specified()
        {
            theLibrary.Always.Build(x => new HtmlTag("div").Text(((FakeSubject)x).Name));
            theLibrary.Category("a").Always.Build(x => new HtmlTag("a").Text(((FakeSubject)x).Name));
            theLibrary.Category("b").Always.Build(x => new HtmlTag("b").Text(((FakeSubject)x).Name));

            theLibrary.ForProfile("profile1").Always.Build(x => new HtmlTag("p").Text(((FakeSubject)x).Name));
            theLibrary.Category("a").ForProfile("a-1").Always.Modify(x => x.CurrentTag.AddClass("a-1"));

            var subject = new FakeSubject { Name = "Lindsey" };

            build(subject).ToString().ShouldEqual("<div>Lindsey</div>");
            build(subject, category:"a").ToString().ShouldEqual("<a>Lindsey</a>");
            build(subject, category:"a", profile:"a-1").ToString().ShouldEqual("<a class=\"a-1\">Lindsey</a>");
            build(subject, category:"b").ToString().ShouldEqual("<b>Lindsey</b>");
            build(subject, profile:"profile1").ToString().ShouldEqual("<p>Lindsey</p>");
        }
    }

    
    public class TagLibrary_Import_Tester
    {
        private ITagBuilderPolicy b1;
        private ITagBuilderPolicy b2;
        private ITagBuilderPolicy b3;
        private ITagBuilderPolicy b4;
        private ITagBuilderPolicy b5;
        private ITagBuilderPolicy b6;
        private ITagBuilderPolicy b7;
        private ITagModifier m1;
        private ITagModifier m2;
        private ITagModifier m3;
        private ITagModifier m4;
        private TagLibrary library1;

        public TagLibrary_Import_Tester()
        {
            b1 = new Mock<ITagBuilderPolicy>().Object;
            b2 = new Mock<ITagBuilderPolicy>().Object;
            b3 = new Mock<ITagBuilderPolicy>().Object;
            b4 = new Mock<ITagBuilderPolicy>().Object;
            b5 = new Mock<ITagBuilderPolicy>().Object;
            b6 = new Mock<ITagBuilderPolicy>().Object;
            b7 = new Mock<ITagBuilderPolicy>().Object;

            m1 = new Mock<ITagModifier>().Object;
            m2 = new Mock<ITagModifier>().Object;
            m3 = new Mock<ITagModifier>().Object;
            m4 = new Mock<ITagModifier>().Object;

            library1 = new TagLibrary();


            library1.Add(b1);
            library1.Add(m1);

            library1.Default.Profile("A").Add(b2);
            library1.Default.Profile("A").Add(m2);

            library1.Category("Cat1").Add(b3);
            library1.Category("Cat1").Add(m3);

            library1.Category("Cat1").Profile("A").Add(b4);


            var library2 = new TagLibrary();
            library2.Add(b5);
            library2.Add(m4);

            library2.Category("Cat2").Add(b6);
            library2.Category("Cat1").Add(b7);

            library1.Import(library2);
        }

        [Fact]
        public void imports_defaults()
        {
            library1.Default.Defaults.Policies.ShouldHaveTheSameElementsAs(b1, b5);
        }

        [Fact]
        public void imports_category_that_both_libraries_have()
        {
            library1.Category("Cat1").Defaults.Policies.ShouldHaveTheSameElementsAs(b3, b7);
        }

        [Fact]
        public void imports_category_from_the_second_library_not_originally_in_the_first()
        {
            library1.Category("Cat2").Defaults.Policies.ShouldHaveTheSameElementsAs(b6);
        }


    }
}
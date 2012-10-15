using System;
using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace HtmlTags.Testing.Conventions
{
    [TestFixture]
    public class TagCategoryTester
    {
        private TagCategory<FakeSubject> theCategory;

        [SetUp]
        public void SetUp()
        {
            theCategory = new TagCategory<FakeSubject>();

            
        }

        private HtmlTag build(FakeSubject subject, string profile = null)
        {
            var plan = theCategory.PlanFor(subject, profile);
            return plan.Build(subject);
        }

        [Test]
        public void build_default_profile_simple()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text(x.Name));

            build(new FakeSubject{
                Name = "Jeremy"
            }).ToString()
                .ShouldEqual("<div>Jeremy</div>");
        }

        [Test]
        public void build_default_profile_where_it_has_to_select_builder()
        {
            theCategory.If(x => x.Level < 10).Build(x => new HtmlTag("h4").Text(x.Name));
            theCategory.If(x => x.Level >= 10).Build(x => new HtmlTag("h1").Text(x.Name));

            build(new FakeSubject{
                Name = "Jeremy",
                Level = 5
            }).ToString().ShouldEqual("<h4>Jeremy</h4>");

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 11
            }).ToString().ShouldEqual("<h1>Jeremy</h1>");
        }

        [Test]
        public void build_default_with_matching_modifiers()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text(x.Name));
            theCategory.If(x => x.Level < 10).Modify(x => x.CurrentTag.AddClass("little"));
            theCategory.If(x => x.Level >= 10).Modify(x => x.CurrentTag.AddClass("big"));

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 5
            }).ToString().ShouldEqual("<div class=\"little\">Jeremy</div>");

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 11
            }).ToString().ShouldEqual("<div class=\"big\">Jeremy</div>");
        }

        [Test]
        public void build_default_profile_with_multiple_modifiers()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text(x.Name));
            theCategory.If(x => x.Level < 10).Modify(x => x.CurrentTag.AddClass("little"));
            theCategory.If(x => x.Level >= 10).Modify(x => x.CurrentTag.AddClass("big"));
            theCategory.Always.Modify(x => x.CurrentTag.AddClass("more"));

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 5
            }).ToString().ShouldEqual("<div class=\"little more\">Jeremy</div>");

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 11
            }).ToString().ShouldEqual("<div class=\"big more\">Jeremy</div>");
        }



        [Test]
        public void build_a_profile_simple()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text("Default"));
            theCategory.ForProfile("A").Always.Build(x => new HtmlTag("div").Text(x.Name));

            build(new FakeSubject
            {
                Name = "Jeremy"
            }, "A").ToString()
                .ShouldEqual("<div>Jeremy</div>");
        }

        [Test]
        public void build_a_profile_where_it_has_to_select_builder()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text("Default"));
            theCategory.ForProfile("A").If(x => x.Level < 10).Build(x => new HtmlTag("h4").Text(x.Name));
            theCategory.ForProfile("A").If(x => x.Level >= 10).Build(x => new HtmlTag("h1").Text(x.Name));

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 5
            }, "A").ToString().ShouldEqual("<h4>Jeremy</h4>");

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 11
            }, "A").ToString().ShouldEqual("<h1>Jeremy</h1>");
        }

        [Test]
        public void build_a_profile_with_matching_modifiers()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text("Default"));
            theCategory.ForProfile("A").Always.Build(x => new HtmlTag("div").Text(x.Name));
            theCategory.ForProfile("A").If(x => x.Level < 10).Modify(x => x.CurrentTag.AddClass("little"));
            theCategory.ForProfile("A").If(x => x.Level >= 10).Modify(x => x.CurrentTag.AddClass("big"));

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 5
            }, "A").ToString().ShouldEqual("<div class=\"little\">Jeremy</div>");

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 11
            }, "A").ToString().ShouldEqual("<div class=\"big\">Jeremy</div>");
        }

        [Test]
        public void build_a_profile_with_multiple_modifiers()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text("Default"));
            theCategory.ForProfile("A").Always.Build(x => new HtmlTag("div").Text(x.Name));
            theCategory.ForProfile("A").If(x => x.Level < 10).Modify(x => x.CurrentTag.AddClass("little"));
            theCategory.ForProfile("A").If(x => x.Level >= 10).Modify(x => x.CurrentTag.AddClass("big"));
            theCategory.ForProfile("A").Always.Modify(x => x.CurrentTag.AddClass("more"));

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 5
            }, "A").ToString().ShouldEqual("<div class=\"little more\">Jeremy</div>");

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 11
            }, "A").ToString().ShouldEqual("<div class=\"big more\">Jeremy</div>");
        }

        [Test]
        public void profile_falls_through_to_default_builder_if_there_is_none_in_the_specific_profile()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text("Default"));
            theCategory.ForProfile("H1").Always.Build(x => new HtmlTag("h1").Text("Default"));

            build(new FakeSubject()).ToString().ShouldEqual("<div>Default</div>");
            build(new FakeSubject(), "H1").ToString().ShouldEqual("<h1>Default</h1>");
            build(new FakeSubject(), "Different").ToString().ShouldEqual("<div>Default</div>");
        }

        [Test]
        public void modifiers_are_isolated_between_non_default_profiles()
        {
            theCategory.Always.Build(x => new HtmlTag("div"));
            theCategory.ForProfile("A").Always.Modify(x => x.CurrentTag.AddClass("A"));
            theCategory.ForProfile("A").Always.Modify(x => x.CurrentTag.AddClass("A1"));
            theCategory.ForProfile("B").Always.Modify(x => x.CurrentTag.AddClass("B"));

            build(new FakeSubject()).ToString().ShouldEqual("<div></div>");
            build(new FakeSubject(), "A").ToString().ShouldEqual("<div class=\"A A1\"></div>");
            build(new FakeSubject(), "B").ToString().ShouldEqual("<div class=\"B\"></div>");
        }

        [Test]
        public void modifiers_in_the_default_profile_apply_to_other_profiles()
        {
            theCategory.Always.Build(x => new HtmlTag("div"));
            theCategory.ForProfile("a").Always.Build(x => new HtmlTag("a"));
            theCategory.ForProfile("b").Always.Build(x => new HtmlTag("b"));
        
            theCategory.Always.Modify(x => x.CurrentTag.AddClass("required"));

            build(new FakeSubject()).ToString().ShouldEqual("<div class=\"required\"></div>");
            build(new FakeSubject(), "a").ToString().ShouldEqual("<a class=\"required\"></a>");
            build(new FakeSubject(), "b").ToString().ShouldEqual("<b class=\"required\"></b>");
        }

        [Test]
        public void building_with_no_matching_builder_throws_exception()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                build(new FakeSubject());
            });
        }

        [Test]
        public void building_with_no_matching_builder_throws_exception_2()
        {
            theCategory.If(x => false).Build(x => new HtmlTag("div"));
            theCategory.If(x => false).Build(x => new HtmlTag("div"));
            theCategory.If(x => false).Build(x => new HtmlTag("div"));

            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                build(new FakeSubject());
            });
        }

        [Test]
        public void tag_plan_is_memoized_by_profile_and_subject()
        {
            var subject1 = new FakeSubject { Name = "Jeremy", Level = 10 };
            var subject2 = new FakeSubject { Name = "Jeremy", Level = 10 };

            subject1.ShouldEqual(subject2);

            theCategory.Always.Build(x => new HtmlTag("div"));
            theCategory.ForProfile("a").Always.Build(x => new HtmlTag("a"));
            theCategory.ForProfile("b").Always.Build(x => new HtmlTag("b"));



            theCategory.PlanFor(subject1).ShouldBeTheSameAs(theCategory.PlanFor(subject2));
            theCategory.PlanFor(subject1, "a").ShouldBeTheSameAs(theCategory.PlanFor(subject2, "a"));
            theCategory.PlanFor(subject1, "b").ShouldBeTheSameAs(theCategory.PlanFor(subject2, "b"));

            theCategory.PlanFor(subject1, "a").ShouldNotBeTheSameAs(theCategory.PlanFor(subject2, "b"));
            theCategory.PlanFor(subject1, "b").ShouldNotBeTheSameAs(theCategory.PlanFor(subject2, "a"));
        }

    }

    [TestFixture]
    public class when_importing_one_category_into_another
    {
        private ITagBuilderPolicy<FakeSubject> b1;
        private ITagBuilderPolicy<FakeSubject> b2;
        private ITagBuilderPolicy<FakeSubject> b3;
        private ITagBuilderPolicy<FakeSubject> b4;
        private ITagBuilderPolicy<FakeSubject> b5;
        private ITagBuilderPolicy<FakeSubject> b6;
        private ITagBuilderPolicy<FakeSubject> b7;
        private ITagBuilderPolicy<FakeSubject> b8;
        private ITagModifier<FakeSubject> m1;
        private ITagModifier<FakeSubject> m2;
        private ITagModifier<FakeSubject> m3;
        private ITagModifier<FakeSubject> m4;
        private ITagModifier<FakeSubject> m5;
        private TagCategory<FakeSubject> category1;

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
            b8 = MockRepository.GenerateMock<ITagBuilderPolicy<FakeSubject>>();

            m1 = MockRepository.GenerateMock<ITagModifier<FakeSubject>>();
            m2 = MockRepository.GenerateMock<ITagModifier<FakeSubject>>();
            m3 = MockRepository.GenerateMock<ITagModifier<FakeSubject>>();
            m4 = MockRepository.GenerateMock<ITagModifier<FakeSubject>>();
            m5 = MockRepository.GenerateMock<ITagModifier<FakeSubject>>();

            category1 = new TagCategory<FakeSubject>();
            category1.Add(b1);
            category1.Add(b2);
            category1.Add(m1);
            category1.Add(m2);

            category1.ForProfile("A").Add(b3);
            category1.ForProfile("A").Add(m3);

            category1.ForProfile("B").Add(b4);

            category1.ForProfile("D").Add(b8);


            var category2 = new TagCategory<FakeSubject>();
            category2.Add(b5);
            category2.Add(m4);
            category2.ForProfile("A").Add(b6);
            category2.ForProfile("C").Add(b7);
            category2.ForProfile("B").Add(m5);


            category1.Import(category2);
        }

        [Test]
        public void should_import_the_default_profile()
        {
            category1.Defaults.Policies.ShouldHaveTheSameElementsAs(b1, b2, b5);
            category1.Defaults.Modifiers.ShouldHaveTheSameElementsAs(m1, m2, m4);
        }

        [Test]
        public void does_not_change_profile_held_by_first_category_but_not_the_second()
        {
            category1.Profile("D").Policies.ShouldHaveTheSameElementsAs(b8);
            category1.Profile("D").Modifiers.Any().ShouldBeFalse();
        }

        [Test]
        public void import_profile_held_by_both_categories()
        {
            category1.Profile("A").Policies.ShouldHaveTheSameElementsAs(b3, b6);
            category1.Profile("B").Policies.ShouldHaveTheSameElementsAs(b4);

            category1.Profile("A").Modifiers.ShouldHaveTheSameElementsAs(m3);
            category1.Profile("B").Modifiers.ShouldHaveTheSameElementsAs(m5);
        }

        [Test]
        public void import_profile_held_by_second_profile_but_not_the_first()
        {
            category1.Profile("C").Policies.ShouldHaveTheSameElementsAs(b7);
        }
    }
}
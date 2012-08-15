using System;
using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;

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
}
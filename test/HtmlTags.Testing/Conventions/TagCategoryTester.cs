using System;
using Shouldly;
using HtmlTags.Conventions;
using Xunit;
using Moq;
using System.Linq;

namespace HtmlTags.Testing.Conventions
{
    
    public class TagCategoryTester
    {
        private TagCategory theCategory;

        public TagCategoryTester()
        {
            theCategory = new TagCategory();

            
        }

        private HtmlTag build(FakeSubject subject, string profile = null)
        {
            var plan = theCategory.PlanFor(subject, profile);
            return plan.Build(subject);
        }

        [Fact]
        public void build_default_profile_simple()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text(((FakeSubject)x).Name));

            build(new FakeSubject{
                Name = "Jeremy"
            }).ToString()
                .ShouldBe("<div>Jeremy</div>");
        }

        [Fact]
        public void build_default_profile_where_it_has_to_select_builder()
        {
            theCategory.If(x => ((FakeSubject)x).Level < 10).Build(x => new HtmlTag("h4").Text(((FakeSubject)x).Name));
            theCategory.If(x => ((FakeSubject)x).Level >= 10).Build(x => new HtmlTag("h1").Text(((FakeSubject)x).Name));

            build(new FakeSubject{
                Name = "Jeremy",
                Level = 5
            }).ToString().ShouldBe("<h4>Jeremy</h4>");

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 11
            }).ToString().ShouldBe("<h1>Jeremy</h1>");
        }

        [Fact]
        public void build_default_with_matching_modifiers()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text(((FakeSubject)x).Name));
            theCategory.If(x => ((FakeSubject)x).Level < 10).Modify(x => x.CurrentTag.AddClass("little"));
            theCategory.If(x => ((FakeSubject)x).Level >= 10).Modify(x => x.CurrentTag.AddClass("big"));

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 5
            }).ToString().ShouldBe("<div class=\"little\">Jeremy</div>");

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 11
            }).ToString().ShouldBe("<div class=\"big\">Jeremy</div>");
        }

        [Fact]
        public void build_default_profile_with_multiple_modifiers()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text(((FakeSubject)x).Name));
            theCategory.If(x => ((FakeSubject)x).Level < 10).Modify(x => x.CurrentTag.AddClass("little"));
            theCategory.If(x => ((FakeSubject)x).Level >= 10).Modify(x => x.CurrentTag.AddClass("big"));
            theCategory.Always.Modify(x => x.CurrentTag.AddClass("more"));

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 5
            }).ToString().ShouldBe("<div class=\"little more\">Jeremy</div>");

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 11
            }).ToString().ShouldBe("<div class=\"big more\">Jeremy</div>");
        }



        [Fact]
        public void build_a_profile_simple()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text("Default"));
            theCategory.ForProfile("A").Always.Build(x => new HtmlTag("div").Text(((FakeSubject)x).Name));

            build(new FakeSubject
            {
                Name = "Jeremy"
            }, "A").ToString()
                .ShouldBe("<div>Jeremy</div>");
        }

        [Fact]
        public void build_a_profile_where_it_has_to_select_builder()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text("Default"));
            theCategory.ForProfile("A").If(x => ((FakeSubject)x).Level < 10).Build(x => new HtmlTag("h4").Text(((FakeSubject)x).Name));
            theCategory.ForProfile("A").If(x => ((FakeSubject)x).Level >= 10).Build(x => new HtmlTag("h1").Text(((FakeSubject)x).Name));

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 5
            }, "A").ToString().ShouldBe("<h4>Jeremy</h4>");

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 11
            }, "A").ToString().ShouldBe("<h1>Jeremy</h1>");
        }

        [Fact]
        public void build_a_profile_with_matching_modifiers()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text("Default"));
            theCategory.ForProfile("A").Always.Build(x => new HtmlTag("div").Text(((FakeSubject)x).Name));
            theCategory.ForProfile("A").If(x => ((FakeSubject)x).Level < 10).Modify(x => x.CurrentTag.AddClass("little"));
            theCategory.ForProfile("A").If(x => ((FakeSubject)x).Level >= 10).Modify(x => x.CurrentTag.AddClass("big"));

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 5
            }, "A").ToString().ShouldBe("<div class=\"little\">Jeremy</div>");

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 11
            }, "A").ToString().ShouldBe("<div class=\"big\">Jeremy</div>");
        }

        [Fact]
        public void build_a_profile_with_multiple_modifiers()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text("Default"));
            theCategory.ForProfile("A").Always.Build(x => new HtmlTag("div").Text(((FakeSubject)x).Name));
            theCategory.ForProfile("A").If(x => ((FakeSubject)x).Level < 10).Modify(x => x.CurrentTag.AddClass("little"));
            theCategory.ForProfile("A").If(x => ((FakeSubject)x).Level >= 10).Modify(x => x.CurrentTag.AddClass("big"));
            theCategory.ForProfile("A").Always.Modify(x => x.CurrentTag.AddClass("more"));

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 5
            }, "A").ToString().ShouldBe("<div class=\"little more\">Jeremy</div>");

            build(new FakeSubject
            {
                Name = "Jeremy",
                Level = 11
            }, "A").ToString().ShouldBe("<div class=\"big more\">Jeremy</div>");
        }

        [Fact]
        public void profile_falls_through_to_default_builder_if_there_is_none_in_the_specific_profile()
        {
            theCategory.Always.Build(x => new HtmlTag("div").Text("Default"));
            theCategory.ForProfile("H1").Always.Build(x => new HtmlTag("h1").Text("Default"));

            build(new FakeSubject()).ToString().ShouldBe("<div>Default</div>");
            build(new FakeSubject(), "H1").ToString().ShouldBe("<h1>Default</h1>");
            build(new FakeSubject(), "Different").ToString().ShouldBe("<div>Default</div>");
        }

        [Fact]
        public void modifiers_are_isolated_between_non_default_profiles()
        {
            theCategory.Always.Build(x => new HtmlTag("div"));
            theCategory.ForProfile("A").Always.Modify(x => x.CurrentTag.AddClass("A"));
            theCategory.ForProfile("A").Always.Modify(x => x.CurrentTag.AddClass("A1"));
            theCategory.ForProfile("B").Always.Modify(x => x.CurrentTag.AddClass("B"));

            build(new FakeSubject()).ToString().ShouldBe("<div></div>");
            build(new FakeSubject(), "A").ToString().ShouldBe("<div class=\"A A1\"></div>");
            build(new FakeSubject(), "B").ToString().ShouldBe("<div class=\"B\"></div>");
        }

        [Fact]
        public void modifiers_in_the_default_profile_apply_to_other_profiles()
        {
            theCategory.Always.Build(x => new HtmlTag("div"));
            theCategory.ForProfile("a").Always.Build(x => new HtmlTag("a"));
            theCategory.ForProfile("b").Always.Build(x => new HtmlTag("b"));
        
            theCategory.Always.Modify(x => x.CurrentTag.AddClass("required"));

            build(new FakeSubject()).ToString().ShouldBe("<div class=\"required\"></div>");
            build(new FakeSubject(), "a").ToString().ShouldBe("<a class=\"required\"></a>");
            build(new FakeSubject(), "b").ToString().ShouldBe("<b class=\"required\"></b>");
        }

        [Fact]
        public void building_with_no_matching_builder_throws_exception()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                build(new FakeSubject());
            });
        }

        [Fact]
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

        [Fact]
        public void tag_plan_is_not_memoized_by_profile_and_subject()
        {
            var subject1 = new FakeSubject { Name = "Jeremy", Level = 10 };
            var subject2 = new FakeSubject { Name = "Jeremy", Level = 10 };

            subject1.ShouldBe(subject2);

            theCategory.Always.Build(x => new HtmlTag("div"));
            theCategory.ForProfile("a").Always.Build(x => new HtmlTag("a"));
            theCategory.ForProfile("b").Always.Build(x => new HtmlTag("b"));



            theCategory.PlanFor(subject1).ShouldNotBeSameAs(theCategory.PlanFor(subject2));
            theCategory.PlanFor(subject1, "a").ShouldNotBeSameAs(theCategory.PlanFor(subject2, "a"));
            theCategory.PlanFor(subject1, "b").ShouldNotBeSameAs(theCategory.PlanFor(subject2, "b"));

            theCategory.PlanFor(subject1, "a").ShouldNotBeSameAs(theCategory.PlanFor(subject2, "b"));
            theCategory.PlanFor(subject1, "b").ShouldNotBeSameAs(theCategory.PlanFor(subject2, "a"));
        }

    }

    
    public class when_importing_one_category_into_another
    {
        private ITagBuilderPolicy b1;
        private ITagBuilderPolicy b2;
        private ITagBuilderPolicy b3;
        private ITagBuilderPolicy b4;
        private ITagBuilderPolicy b5;
        private ITagBuilderPolicy b6;
        private ITagBuilderPolicy b7;
        private ITagBuilderPolicy b8;
        private ITagModifier m1;
        private ITagModifier m2;
        private ITagModifier m3;
        private ITagModifier m4;
        private ITagModifier m5;
        private TagCategory category1;

        public when_importing_one_category_into_another()
        {
            b1 = new Mock<ITagBuilderPolicy>().Object;
            b2 = new Mock<ITagBuilderPolicy>().Object;
            b3 = new Mock<ITagBuilderPolicy>().Object;
            b4 = new Mock<ITagBuilderPolicy>().Object;
            b5 = new Mock<ITagBuilderPolicy>().Object;
            b6 = new Mock<ITagBuilderPolicy>().Object;
            b7 = new Mock<ITagBuilderPolicy>().Object;
            b8 = new Mock<ITagBuilderPolicy>().Object;

            m1 = new Mock<ITagModifier>().Object;
            m2 = new Mock<ITagModifier>().Object;
            m3 = new Mock<ITagModifier>().Object;
            m4 = new Mock<ITagModifier>().Object;
            m5 = new Mock<ITagModifier>().Object;

            category1 = new TagCategory();
            category1.Add(b1);
            category1.Add(b2);
            category1.Add(m1);
            category1.Add(m2);

            category1.ForProfile("A").Add(b3);
            category1.ForProfile("A").Add(m3);

            category1.ForProfile("B").Add(b4);

            category1.ForProfile("D").Add(b8);


            var category2 = new TagCategory();
            category2.Add(b5);
            category2.Add(m4);
            category2.ForProfile("A").Add(b6);
            category2.ForProfile("C").Add(b7);
            category2.ForProfile("B").Add(m5);


            category1.Import(category2);
        }

        [Fact]
        public void should_import_the_default_profile()
        {
            category1.Defaults.Policies.ShouldHaveTheSameElementsAs(b1, b2, b5);
            category1.Defaults.Modifiers.ShouldHaveTheSameElementsAs(m1, m2, m4);
        }

        [Fact]
        public void does_not_change_profile_held_by_first_category_but_not_the_second()
        {
            category1.Profile("D").Policies.ShouldHaveTheSameElementsAs(b8);
            category1.Profile("D").Modifiers.Any().ShouldBeFalse();
        }

        [Fact]
        public void import_profile_held_by_both_categories()
        {
            category1.Profile("A").Policies.ShouldHaveTheSameElementsAs(b3, b6);
            category1.Profile("B").Policies.ShouldHaveTheSameElementsAs(b4);

            category1.Profile("A").Modifiers.ShouldHaveTheSameElementsAs(m3);
            category1.Profile("B").Modifiers.ShouldHaveTheSameElementsAs(m5);
        }

        [Fact]
        public void import_profile_held_by_second_profile_but_not_the_first()
        {
            category1.Profile("C").Policies.ShouldHaveTheSameElementsAs(b7);
        }
    }
}
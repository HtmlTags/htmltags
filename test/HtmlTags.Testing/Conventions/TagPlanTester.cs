using Should;
using HtmlTags.Conventions;
using Xunit;

namespace HtmlTags.Testing.Conventions
{
    using HtmlTags.Conventions.Elements;

    
    public class TagPlanTester
    {
        [Fact]
        public void build_tag_with_multiple_modifiers()
        {
            var plan = new TagPlan(new ByNameBuilder(),
                new ITagModifier[] {new FakeAddClass(1, "a"), new FakeAddClass(2, "b")},
                new DefaultElementNamingConvention());

            plan.Build(new FakeSubject{
                Name = "Malcolm Reynolds"
            })
                .ToString().ShouldEqual("<div id=\"Malcolm Reynolds\" class=\"a b\"></div>");
        }

        [Fact]
        public void build_tag_with_wrapper()
        {
            var plan = new TagPlan(new ByNameBuilder(),
                new ITagModifier[] {new FakeAddClass(1, "a"), new FakeAddClass(2, "b"), new WrapWithDiv()},
                new DefaultElementNamingConvention());

            plan.Build(new FakeSubject
            {
                Name = "Malcolm Reynolds"
            })
                .ToString().ShouldEqual("<div class=\"wrapper\"><div id=\"Malcolm Reynolds\" class=\"a b\"></div></div>");
        }
    }

    public class WrapWithDiv : ITagModifier
    {
        public bool Matches(ElementRequest token)
        {
            return true;
        }

        public void Modify(ElementRequest request)
        {
            request.WrapWith(new HtmlTag("div").AddClass("wrapper"));
        }
    }

}
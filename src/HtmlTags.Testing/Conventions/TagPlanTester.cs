using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;

namespace HtmlTags.Testing.Conventions
{
    [TestFixture]
    public class TagPlanTester
    {
        [Test]
        public void build_tag_with_multiple_modifiers()
        {
            var plan = new TagPlan(new ByNameBuilder(),
                                                new ITagModifier[]
                                                {new FakeAddClass(1, "a"), new FakeAddClass(2, "b")});

            plan.Build(new FakeSubject{
                Name = "Malcolm Reynolds"
            })
                .ToString().ShouldEqual("<div id=\"Malcolm Reynolds\" class=\"a b\"></div>");
        }

        [Test]
        public void build_tag_with_wrapper()
        {
            var plan = new TagPlan(new ByNameBuilder(),
                                                new ITagModifier[] { new FakeAddClass(1, "a"), new FakeAddClass(2, "b"), new WrapWithDiv() });

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
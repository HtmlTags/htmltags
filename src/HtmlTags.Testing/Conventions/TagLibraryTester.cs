using HtmlTags.Conventions;
using NUnit.Framework;
using FubuTestingSupport;

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
            theLibrary.ForCategory("a").Always.Build(x => new HtmlTag("a").Text(x.Name));
            theLibrary.ForCategory("b").Always.Build(x => new HtmlTag("b").Text(x.Name));
        
            theLibrary.ForProfile("profile1").Always.Build(x => new HtmlTag("p").Text(x.Name));
            theLibrary.ForCategory("a").ForProfile("a-1").Always.Modify(x => x.CurrentTag.AddClass("a-1"));

            var subject = new FakeSubject { Name = "Lindsey" };

            build(subject).ToString().ShouldEqual("<div>Lindsey</div>");
            build(subject, category:"a").ToString().ShouldEqual("<a>Lindsey</a>");
            build(subject, category:"a", profile:"a-1").ToString().ShouldEqual("<a class=\"a-1\">Lindsey</a>");
            build(subject, category:"b").ToString().ShouldEqual("<b>Lindsey</b>");
            build(subject, profile:"profile1").ToString().ShouldEqual("<p>Lindsey</p>");
        }
    }
}
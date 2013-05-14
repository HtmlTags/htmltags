using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;
using Rhino.Mocks;

namespace HtmlTags.Testing.Conventions
{
    [TestFixture]
    public class TagGeneratorTester : InteractionContext<TagGenerator<FakeSubject>>
    {
        private FakeSubject theSubject;
        private HtmlTag theTag;

        protected override void beforeEach()
        {

            theSubject = new FakeSubject{
                Name = "Jeremy",
                Level = 10
            };

            theTag = new HtmlTag("div");
        }

        private void expect(FakeSubject subject, string category = null, string profile = null)
        {
            var thePlan = MockFor<ITagPlan<FakeSubject>>();

            MockFor<ITagLibrary<FakeSubject>>().Stub(x => x.PlanFor(subject, profile: profile, category: category))
                .Return(thePlan);


            thePlan.Stub(x => x.Build(theSubject)).Return(theTag);

            MockFor<ITagRequestBuilder>().Stub(x => x.Build(theSubject)).IgnoreArguments();
        }

        [Test]
        public void the_default_profile_is_Default()
        {
            ClassUnderTest.ActiveProfile.ShouldEqual(TagConstants.Default);
        }

        [Test]
        public void ensure_tag_requests_are_always_built()
        {
            expect(theSubject, TagConstants.Default, TagConstants.Default);

            ClassUnderTest.Build(theSubject);

            MockFor<ITagRequestBuilder>().AssertWasCalled(x => x.Build(theSubject));
            
        }

        [Test]
        public void call_build_in_default_mode()
        {
            expect(theSubject, category:TagConstants.Default, profile:TagConstants.Default);

            ClassUnderTest.Build(theSubject).ShouldBeTheSameAs(theTag);
        }

        [Test]
        public void call_build_with_the_profile_set()
        {
            MockFor<ActiveProfile>().Push("A");

            expect(theSubject, category: TagConstants.Default, profile: "A");

            ClassUnderTest.Build(theSubject).ShouldBeTheSameAs(theTag);
        }

        [Test]
        public void call_build_with_category()
        {
            expect(theSubject, category:"A", profile:TagConstants.Default);

            ClassUnderTest.Build(theSubject, "A", null).ShouldBeTheSameAs(theTag);
        }

        [Test]
        public void call_build_with_both_category_and_non_default_profile()
        {
            MockFor<ActiveProfile>().Push("B");

            expect(theSubject, category:"A", profile:"B");

            ClassUnderTest.Build(theSubject, "A", null).ShouldBeTheSameAs(theTag);
        }

        [Test]
        public void call_build_with_both_category_and_non_default_profile_by_passing_in_the_default()
        {
            expect(theSubject, category: "A", profile: "B");

            ClassUnderTest.Build(theSubject, "A", "B").ShouldBeTheSameAs(theTag);
        }
    }
}
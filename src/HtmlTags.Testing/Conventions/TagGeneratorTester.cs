using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;
using Rhino.Mocks;

namespace HtmlTags.Testing.Conventions
{
    [TestFixture]
    public class TagGeneratorTester : InteractionContext<TagGenerator<FakeSubject>>
    {
        private ITagRequestActivator[] theActivators;
        private FakeSubject theSubject;
        private HtmlTag theTag;

        protected override void beforeEach()
        {
            theActivators = Services.CreateMockArrayFor<ITagRequestActivator>(5);

            theActivators[0].Stub(x => x.Matches(typeof (FakeSubject))).Return(true);    
            theActivators[1].Stub(x => x.Matches(typeof (FakeSubject))).Return(false);    
            theActivators[2].Stub(x => x.Matches(typeof (FakeSubject))).Return(true);    
            theActivators[3].Stub(x => x.Matches(typeof (FakeSubject))).Return(false);    
            theActivators[4].Stub(x => x.Matches(typeof (FakeSubject))).Return(true);

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
        }

        [Test]
        public void the_default_profile_is_Default()
        {
            ClassUnderTest.ActiveProfile.ShouldEqual(TagConstants.Default);
        }

        [Test]
        public void only_calls_the_activators_that_match_on_the_subject_type()
        {
            expect(theSubject, TagConstants.Default, TagConstants.Default);

            ClassUnderTest.Build(theSubject);

            theActivators[0].AssertWasCalled(x => x.Activate(theSubject));
            theActivators[1].AssertWasNotCalled(x => x.Activate(theSubject));
            theActivators[2].AssertWasCalled(x => x.Activate(theSubject));
            theActivators[3].AssertWasNotCalled(x => x.Activate(theSubject));
            theActivators[4].AssertWasCalled(x => x.Activate(theSubject));
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
            ClassUnderTest.ActiveProfile = "A";

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
            ClassUnderTest.ActiveProfile = "B";

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
using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;
using Rhino.Mocks;

namespace HtmlTags.Testing.Conventions
{
    [TestFixture]
    public class TagRequestBuilderTester : InteractionContext<TagRequestBuilder>
    {
        private ITagRequestActivator[] theActivators;
        private FakeSubject theSubject;

        protected override void beforeEach()
        {
           theActivators = Services.CreateMockArrayFor<ITagRequestActivator>(5);

            theActivators[0].Stub(x => x.Matches(typeof(FakeSubject))).Return(true);
            theActivators[1].Stub(x => x.Matches(typeof(FakeSubject))).Return(false);
            theActivators[2].Stub(x => x.Matches(typeof(FakeSubject))).Return(true);
            theActivators[3].Stub(x => x.Matches(typeof(FakeSubject))).Return(false);
            theActivators[4].Stub(x => x.Matches(typeof(FakeSubject))).Return(true);

            theSubject = new FakeSubject()
                {
                    Name = "HerpDerp",
                    Level = 99
                };
        }

        [Test]
        public void should_only_call_activators_that_match()
        {
            ClassUnderTest.Build(theSubject);

             theActivators[0].AssertWasCalled(x => x.Activate(theSubject));
             theActivators[1].AssertWasNotCalled(x => x.Activate(theSubject));
             theActivators[2].AssertWasCalled(x => x.Activate(theSubject));
             theActivators[3].AssertWasNotCalled(x => x.Activate(theSubject));
             theActivators[4].AssertWasCalled(x => x.Activate(theSubject));
        }
    }
}
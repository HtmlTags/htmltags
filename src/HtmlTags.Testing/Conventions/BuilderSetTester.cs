using HtmlTags.Conventions;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace HtmlTags.Testing.Conventions
{
    using Reflection;

    [TestFixture]
    public class BuilderSetTester
    {
        [Test]
        public void import_puts_the_second_set_stuff_in_the_back()
        {
            var builder1 = MockRepository.GenerateMock<ITagBuilderPolicy>();
            var builder2 = MockRepository.GenerateMock<ITagBuilderPolicy>();
            var builder3 = MockRepository.GenerateMock<ITagBuilderPolicy>();

            var m1 = MockRepository.GenerateMock<ITagModifier>();
            var m2 = MockRepository.GenerateMock<ITagModifier>();
            var m3 = MockRepository.GenerateMock<ITagModifier>();
            var m4 = MockRepository.GenerateMock<ITagModifier>();
            var m5 = MockRepository.GenerateMock<ITagModifier>();

            var set1 = new BuilderSet();
            set1.Add(builder1);
            set1.Add(m1);
            set1.Add(m2);
            set1.Add(m3);

            var set2 = new BuilderSet();
            set2.Add(builder2);
            set2.Add(builder3);
            set2.Add(m4);
            set2.Add(m5);

            set1.Import(set2);

            set1.Policies.ShouldHaveTheSameElementsAs(builder1, builder2, builder3);
            set1.Modifiers.ShouldHaveTheSameElementsAs(m1, m2, m3, m4, m5);
        }

        [Test]
        public void insert_builder()
        {
            var builder1 = MockRepository.GenerateMock<ITagBuilderPolicy>();
            var builder2 = MockRepository.GenerateMock<ITagBuilderPolicy>();
            var builder3 = MockRepository.GenerateMock<ITagBuilderPolicy>();

            var set1 = new BuilderSet();
            set1.Add(builder2);
            set1.Add(builder3);

            set1.InsertFirst(builder1);
            set1.Policies.ShouldHaveTheSameElementsAs(builder1, builder2, builder3);

        }
    }

    public class SomethingSubject : ElementRequest
    {
        public int Level { get; set; }

        public SomethingSubject(Accessor accessor) : base(accessor)
        {
        }
    }

}
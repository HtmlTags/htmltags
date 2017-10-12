using HtmlTags.Conventions;
using Moq;
using Xunit;
using Shouldly;

namespace HtmlTags.Testing.Conventions
{
    using Reflection;

    
    public class BuilderSetTester
    {
        [Fact]
        public void import_puts_the_second_set_stuff_in_the_back()
        {
            var builder1 = new Mock<ITagBuilderPolicy>().Object;
            var builder2 = new Mock<ITagBuilderPolicy>().Object;
            var builder3 = new Mock<ITagBuilderPolicy>().Object;

            var m1 = new Mock<ITagModifier>().Object;
            var m2 = new Mock<ITagModifier>().Object;
            var m3 = new Mock<ITagModifier>().Object;
            var m4 = new Mock<ITagModifier>().Object;
            var m5 = new Mock<ITagModifier>().Object;

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

        [Fact]
        public void insert_builder()
        {
            var builder1 = new Mock<ITagBuilderPolicy>().Object;
            var builder2 = new Mock<ITagBuilderPolicy>().Object;
            var builder3 = new Mock<ITagBuilderPolicy>().Object;

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
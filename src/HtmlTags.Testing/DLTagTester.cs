using FubuTestingSupport;
using NUnit.Framework;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class DLTagTester
    {
        [Test]
        public void creates_a_definition_list()
        {
            new DLTag().ToString().ShouldEqual("<dl></dl>");
        }

        [Test]
        public void create_and_initialize_a_definition_list()
        {
            new DLTag(x => x.Id("books")).ToString().ShouldEqual("<dl id=\"books\"></dl>");
        }

        [Test]
        public void AddDefinition_adds_a_term_and_definition_to_the_list()
        {
            new DLTag().AddDefinition("TX", "Texas").ToString().ShouldEqual("<dl><dt>TX</dt><dd>Texas</dd></dl>");
        }
    }
}
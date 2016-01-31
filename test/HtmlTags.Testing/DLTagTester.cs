using Should;
using Xunit;

namespace HtmlTags.Testing
{
    
    public class DLTagTester
    {
        [Fact]
        public void creates_a_definition_list()
        {
            new DLTag().ToString().ShouldEqual("<dl></dl>");
        }

        [Fact]
        public void create_and_initialize_a_definition_list()
        {
            new DLTag(x => x.Id("books")).ToString().ShouldEqual("<dl id=\"books\"></dl>");
        }

        [Fact]
        public void AddDefinition_adds_a_term_and_definition_to_the_list()
        {
            new DLTag().AddDefinition("TX", "Texas").ToString().ShouldEqual("<dl><dt>TX</dt><dd>Texas</dd></dl>");
        }
    }
}
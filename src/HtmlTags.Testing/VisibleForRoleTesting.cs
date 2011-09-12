using System.Security.Principal;
using System.Threading;
using FubuTestingSupport;
using NUnit.Framework;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class VisibleForRoleTesting
    {
        private GenericPrincipal principal;

        [SetUp]
        public void SetUp()
        {
            principal = new GenericPrincipal(new GenericIdentity("somebody"), new string[]{"role1", "role2", "role3"});
            Thread.CurrentPrincipal = principal;
        }

        [Test]
        public void should_be_visible_when_the_user_has_the_role()
        {
            var tag = new HtmlTag("span").VisibleForRoles("role1", "role4");
            tag.Render().ShouldBeTrue();
        }

        [Test]
        public void should_not_be_visible_when_the_user_does_not_have_the_role()
        {
            var tag = new HtmlTag("span").VisibleForRoles("role10", "role4");
            tag.Render().ShouldBeFalse(); 
        }
    }
}
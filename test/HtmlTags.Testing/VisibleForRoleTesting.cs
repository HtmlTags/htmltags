using System.Security.Principal;
using System.Threading;
using Shouldly;
using Xunit;

namespace HtmlTags.Testing
{
    
    public class VisibleForRoleTesting
    {
        private GenericPrincipal principal;

        public VisibleForRoleTesting()
        {
            principal = new GenericPrincipal(new GenericIdentity("somebody"), new string[]{"role1", "role2", "role3"});
        }

        [Fact]
        public void should_be_visible_when_the_user_has_the_role()
        {
            var tag = new HtmlTag("span").VisibleForRoles(principal, "role1", "role4");
            tag.Render().ShouldBeTrue();
        }

        [Fact]
        public void should_not_be_visible_when_the_user_does_not_have_the_role()
        {
            var tag = new HtmlTag("span").VisibleForRoles(principal, "role10", "role4");
            tag.Render().ShouldBeFalse(); 
        }
    }
}
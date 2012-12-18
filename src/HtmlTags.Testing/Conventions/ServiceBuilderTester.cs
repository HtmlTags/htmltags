using HtmlTags.Conventions;
using NUnit.Framework;
using FubuTestingSupport;

namespace HtmlTags.Testing.Conventions
{
    [TestFixture]
    public class ServiceBuilderTester
    {
        [Test]
        public void fill_into_will_not_overwrite_the_parent_if_it_exists()
        {
            var services1 = new ServiceBuilder();
            var services2 = new ServiceBuilder();
        
        
            services1.Add<IChrome>(() => new AChrome());
            services2.Add<IChrome>(() => new BChrome());

            services2.FillInto(services1);

            services1.Build<IChrome>().ShouldBeOfType<AChrome>();
        }
    }

    public interface IChrome{}
    public class AChrome : IChrome{}
    public class BChrome : IChrome{}
}
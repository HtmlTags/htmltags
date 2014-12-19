namespace HtmlTags.UI
{
    using System;
    using Conventions;

    public class ServiceLocatorTagRequestActivator : ITagRequestActivator
    {
        private readonly IServiceLocator _services;

        public ServiceLocatorTagRequestActivator(IServiceLocator services)
        {
            _services = services;
        }

        public bool Matches(Type requestType)
        {
            return requestType.CanBeCastTo<IServiceLocatorAware>();
        }

        public void Activate(TagRequest request)
        {
            request.As<IServiceLocatorAware>().Attach(_services);
        }
    }

    public interface IServiceLocatorAware
    {
        void Attach(IServiceLocator locator);
    }
}
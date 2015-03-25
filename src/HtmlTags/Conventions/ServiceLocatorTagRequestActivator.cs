namespace HtmlTags.Conventions
{
    using System;

    public class ServiceLocatorTagRequestActivator
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

        public void Activate(ElementRequest request)
        {
            request.Attach(_services);
        }
    }

    public interface IServiceLocatorAware
    {
        void Attach(IServiceLocator locator);
    }
}
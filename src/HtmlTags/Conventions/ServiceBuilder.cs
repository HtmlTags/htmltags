using System;

namespace HtmlTags.Conventions
{
    public class ServiceBuilder
    {
        private readonly Cache<Type, object> _services = new Cache<Type, object>();

        public bool Has(Type type)
        {
            return _services.Has(type);
        }

        public T Build<T>()
        {
            return ((Func<T>) _services[typeof (T)])();
        }

        public void Add<T>(Func<T> func)
        {
            _services[typeof (T)] = func;
        }

        public void FillInto(ServiceBuilder serviceBuilder)
        {
            _services.Each((type, o) => {
                serviceBuilder._services.Fill(type, o);
            });
        }
    }
}
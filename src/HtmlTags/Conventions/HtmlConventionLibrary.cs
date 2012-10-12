using System;
using System.Linq;

namespace HtmlTags.Conventions
{
    public class HtmlConventionLibrary
    {
        private readonly Cache<Type, object> _libraries = new Cache<Type, object>();
        private readonly Cache<string, ServiceBuilder> _services = new Cache<string, ServiceBuilder>(key => new ServiceBuilder());
        private readonly ServiceBuilder _defaultBuilder;

        public HtmlConventionLibrary()
        {
            _libraries.OnMissing = type =>
            {
                var libType = typeof (TagLibrary<>).MakeGenericType(type);
                return Activator.CreateInstance(libType);
            };

            _defaultBuilder = _services[TagConstants.Default];
        }

        public T Get<T>(string profile = null)
        {
            profile = profile ?? TagConstants.Default;
            var builder = _services[profile];
            if (builder.Has(typeof(T))) return builder.Build<T>();

            if (profile != TagConstants.Default && _defaultBuilder.Has(typeof(T)))
            {
                return _defaultBuilder.Build<T>();
            }

            throw new ArgumentOutOfRangeException("T","No service implementation is registered for type " + typeof(T).FullName);
        }

        public void RegisterService<T, TImplementation>(string profile = null) where TImplementation : T, new()
        {
            RegisterService<T>(() => new TImplementation(), profile);
        }

        public void RegisterService<T>(Func<T> builder, string profile = null)
        {
            profile = profile ?? TagConstants.Default;
            _services[profile].Add(builder);
        }

        public TagLibrary<T> For<T>() where T : TagRequest
        {
            return (TagLibrary<T>) _libraries[typeof (T)];
        }

        public void Import(HtmlConventionLibrary library)
        {
            var types = library._libraries.GetKeys().Union(_libraries.GetKeys()).Distinct();
            types
                .Select(t => typeof(HtmlConventionLibraryImporter<>).MakeGenericType(t))
                .Select(t => (IHtmlConventionLibraryImporter)Activator.CreateInstance(t))
                .Each(x => x.Import(this, library));

            library._services.Each((key, builder) => builder.FillInto(_services[key]));
        }
    }
}
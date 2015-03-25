using System;
using System.Linq;

namespace HtmlTags.Conventions
{
    public class HtmlConventionLibrary
    {
        // TODO: Collapse into one library
        private readonly Cache<string, ServiceBuilder> _services = new Cache<string, ServiceBuilder>(key => new ServiceBuilder());
        private readonly ServiceBuilder _defaultBuilder;

        public HtmlConventionLibrary()
        {
            TagLibrary = new TagLibrary();

            _defaultBuilder = _services[TagConstants.Default];
        }

        public TagLibrary TagLibrary { get; private set; }

        public void AcceptVisitor(IHtmlConventionVisitor visitor)
        {
            TagLibrary.AcceptVisitor(visitor);
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

            throw new ArgumentOutOfRangeException("T", "No service implementation is registered for type " + typeof(T).FullName);
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

        public void Import(HtmlConventionLibrary library)
        {
            TagLibrary.Import(library.TagLibrary);
            library._services.Each((key, builder) => builder.FillInto(_services[key]));
        }
    }
}
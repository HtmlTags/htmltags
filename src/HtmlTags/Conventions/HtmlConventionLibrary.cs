using System;
using System.Linq;

namespace HtmlTags.Conventions
{
    public class HtmlConventionLibrary
    {
        private readonly Cache<Type, object> _libraries = new Cache<Type, object>();

        public HtmlConventionLibrary()
        {
            _libraries.OnMissing = type =>
            {
                var libType = typeof (TagLibrary<>).MakeGenericType(type);
                return Activator.CreateInstance(libType);
            };
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
        }

        
    }

    public interface IHtmlConventionLibraryImporter
    {
        void Import(HtmlConventionLibrary target, HtmlConventionLibrary source);
    }

    public class HtmlConventionLibraryImporter<T> : IHtmlConventionLibraryImporter where T : TagRequest
    {
        public void Import(HtmlConventionLibrary target, HtmlConventionLibrary source)
        {
            target.For<T>().Import(source.For<T>());
        }
    }
}
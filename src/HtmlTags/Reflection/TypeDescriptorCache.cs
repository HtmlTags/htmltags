using System.Linq;

namespace HtmlTags.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public interface ITypeDescriptorCache
    {
        IDictionary<string, PropertyInfo> GetPropertiesFor<T>();
        IDictionary<string, PropertyInfo> GetPropertiesFor(Type itemType);
        void ForEachProperty(Type itemType, Action<PropertyInfo> action);
        void ClearAll();
    }

    public class TypeDescriptorCache : ITypeDescriptorCache
    {
        private static readonly Cache<Type, IDictionary<string, PropertyInfo>> _cache;

        static TypeDescriptorCache()
        {
            _cache = new Cache<Type, IDictionary<string, PropertyInfo>>(type => 
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(propertyInfo => propertyInfo.CanWrite)
            .ToDictionary(propertyInfo => propertyInfo.Name));
        }

        public IDictionary<string, PropertyInfo> GetPropertiesFor<T>() => GetPropertiesFor(typeof (T));

        public IDictionary<string, PropertyInfo> GetPropertiesFor(Type itemType) => _cache[itemType];

        public void ForEachProperty(Type itemType, Action<PropertyInfo> action) => _cache[itemType].Values.Each(action);

        public void ClearAll() => _cache.ClearAll();

        public static PropertyInfo GetPropertyFor(Type modelType, string propertyName)
        {
            var dict = _cache[modelType];
            return dict.ContainsKey(propertyName) ? dict[propertyName] : null;
        }
    }
}
namespace HtmlTags.Conventions.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class Stringifier
	{
        private readonly List<PropertyOverrideStrategy> _overrides = new List<PropertyOverrideStrategy>();
        private readonly List<StringifierStrategy> _strategies = new List<StringifierStrategy>();

        private Func<GetStringRequest, string> findConverter(GetStringRequest request)
        {
            if (request.PropertyType.IsNullable())
            {
                if (request.RawValue == null) return r => string.Empty;

                return findConverter(request.GetRequestForNullableType());
            }

            if (request.PropertyType.IsArray)
            {
                if (request.RawValue == null) return r => string.Empty;

                return r =>
                {
                    if (r.RawValue == null) return string.Empty;

                    return r.RawValue.As<Array>().OfType<object>().Select(GetString).Join(", ");
                };
            }        

            StringifierStrategy strategy = _strategies.FirstOrDefault(x => x.Matches(request));
            return strategy == null ? ToString : strategy.StringFunction;
        }

        private static string ToString(GetStringRequest value) => value.RawValue?.ToString() ?? string.Empty;

        public string GetString(GetStringRequest request)
        {
            if (request?.RawValue == null || request.RawValue as string == string.Empty)
                return string.Empty;
            PropertyOverrideStrategy propertyOverride = _overrides.FirstOrDefault(o => o.Matches(request.Property));

            if (propertyOverride != null)
            {
                return propertyOverride.StringFunction(request);
            }

            return findConverter(request)(request);
        }


        public string GetString(object rawValue)
        {
            if (rawValue == null || (rawValue as string) == string.Empty) return string.Empty;

            return GetString(new GetStringRequest(null, rawValue, null, null, null));
        }


        public void AddStrategy(StringifierStrategy strategy) => _strategies.Add(strategy);

        #region Nested type: PropertyOverrideStrategy

        public class PropertyOverrideStrategy
        {
            public Func<PropertyInfo, bool> Matches;
            public Func<GetStringRequest, string> StringFunction;
        }

        #endregion
    }
}
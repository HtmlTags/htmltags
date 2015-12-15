namespace HtmlTags.Conventions.Elements
{
    using System;
    using System.Linq;
    using Reflection;

    public class DotNotationElementNamingConvention : IElementNamingConvention
    {
        public static Func<string, bool> IsCollectionIndexer = x => x.StartsWith("[") && x.EndsWith("]");

        public string GetName(Type modelType, Accessor accessor)
        {
            return accessor.PropertyNames
                .Aggregate((x, y) =>
                {
                    var formatString = IsCollectionIndexer(y)
                                           ? "{0}{1}"
                                           : "{0}.{1}";
                    return string.Format(formatString, new[] {x, y});
                });
        }
    }
}
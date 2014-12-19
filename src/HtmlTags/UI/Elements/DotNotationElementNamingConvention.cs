using System;
using System.Linq;
using HtmlTags.Reflection;

namespace HtmlTags.UI.Elements
{
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
                    return formatString.ToFormat(x, y);
                });
        }
    }
}
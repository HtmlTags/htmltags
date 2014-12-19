using System;
using HtmlTags.Reflection;

namespace HtmlTags.UI.Elements
{
    public class DefaultElementNamingConvention : IElementNamingConvention
    {
        public string GetName(Type modelType, Accessor accessor)
        {
            return accessor.Name;
        }
    }
}
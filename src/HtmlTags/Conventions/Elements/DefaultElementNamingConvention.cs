namespace HtmlTags.Conventions.Elements
{
    using System;
    using Reflection;

    public class DefaultElementNamingConvention : IElementNamingConvention
    {
        public string GetName(Type modelType, Accessor accessor) => accessor.Name;
    }
}
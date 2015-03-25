namespace HtmlTags.Conventions.Elements
{
    using System;
    using Reflection;

    public interface IElementNamingConvention
    {
        string GetName(Type modelType, Accessor accessor);
    }
}
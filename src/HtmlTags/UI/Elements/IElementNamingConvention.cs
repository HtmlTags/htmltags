using System;
using HtmlTags.Reflection;

namespace HtmlTags.UI.Elements
{
    public interface IElementNamingConvention
    {
        string GetName(Type modelType, Accessor accessor);
    }
}
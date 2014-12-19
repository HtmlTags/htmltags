using System;
using System.Linq.Expressions;
using HtmlTags;

namespace HtmlTags.UI.Elements
{
    public interface IElementGenerator<T> where T : class
    {
        T Model { get; set; }
        HtmlTag LabelFor(Expression<Func<T, object>> expression, string profile = null, T model = null);
        HtmlTag InputFor(Expression<Func<T, object>> expression, string profile = null, T model = null);
        HtmlTag DisplayFor(Expression<Func<T, object>> expression, string profile = null, T model = null);

        HtmlTag LabelFor(ElementRequest request, string profile = null, T model = null);
        HtmlTag InputFor(ElementRequest request, string profile = null, T model = null);
        HtmlTag DisplayFor(ElementRequest request, string profile = null, T model = null);
    }
}
namespace HtmlTags.Conventions.Elements
{
    using System;
    using System.Linq.Expressions;

    public interface IElementGenerator<T> where T : class
    {
        T Model { get; set; }
        HtmlTag LabelFor<TResult>(Expression<Func<T, TResult>> expression, string profile = null, T model = null);
        HtmlTag InputFor<TResult>(Expression<Func<T, TResult>> expression, string profile = null, T model = null);
        HtmlTag DisplayFor<TResult>(Expression<Func<T, TResult>> expression, string profile = null, T model = null);
        HtmlTag TagFor<TResult>(Expression<Func<T, TResult>> expression, string category, string profile = null, T model = null);

        HtmlTag LabelFor(ElementRequest request, string profile = null, T model = null);
        HtmlTag InputFor(ElementRequest request, string profile = null, T model = null);
        HtmlTag DisplayFor(ElementRequest request, string profile = null, T model = null);
        HtmlTag TagFor(ElementRequest request, string category, string profile = null, T model = null);
    }
}
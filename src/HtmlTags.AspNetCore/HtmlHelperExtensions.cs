namespace HtmlTags
{
    using System;
    using System.Linq.Expressions;
    using Conventions;
    using Conventions.Elements;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.DependencyInjection;

    public static class HtmlHelperExtensions
    {
        public static HtmlTag Input<T>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression)
            where T : class
        {
            var generator = GetGenerator(helper);
            return generator.InputFor(expression);
        }

        public static HtmlTag Label<T>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression)
            where T : class
        {
            var generator = GetGenerator(helper);
            return generator.LabelFor(expression);
        }

        public static HtmlTag Display<T>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression)
            where T : class
        {
            var generator = GetGenerator(helper);
            return generator.DisplayFor(expression);
        }

        public static HtmlTag Tag<T>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression, string category)
            where T : class
        {
            var generator = GetGenerator(helper);
            return generator.TagFor(expression, category);
        }

        public static IElementGenerator<T> GetGenerator<T>(IHtmlHelper<T> helper) where T : class
        {
            var library = helper.ViewContext.HttpContext.RequestServices.GetService<HtmlConventionLibrary>();
            return ElementGenerator<T>.For(library, t => helper.ViewContext.HttpContext.RequestServices.GetService(t), helper.ViewData.Model);
        }

    }
}
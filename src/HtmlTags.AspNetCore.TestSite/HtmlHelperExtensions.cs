using Microsoft.AspNetCore.Html;

namespace HtmlTags.AspNetCore.TestSite
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class HtmlHelperExtensions
    {
        public static HtmlTag Lookup(this IHtmlHelper helper,
            string lookupBy)
        {
            return new HtmlTag("span")
                    .AddClass("readonly")
                    .AppendHtml("&mdash;")
                    .Data("lookupby", lookupBy)
                ;
        }

        public static HtmlTag PrimaryButton(this IHtmlHelper helper, 
            string text)
        {
            return new HtmlTag("button")
                .Attr("type", "submit")
                .AddClasses("btn", "btn-primary")
                .Text(text);
        }

        public static HtmlTag LinkButton(this IHtmlHelper helper, 
            string text, string url)
        {
            return new HtmlTag("a")
                .Attr("href", url)
                .Attr("role", "button")
                .AddClasses("btn", "btn-default")
                .Text(text);
        }

        public static HtmlTag ButtonGroup(this IHtmlHelper helper, 
            params HtmlTag[] buttons)
        {
            var outer = new HtmlTag("div").AddClass("form-group");
            var inner = new HtmlTag("div")
                .AddClasses("col-md-offset-2", "col-md-10")
                .Append(buttons);

            return outer.Append(inner);
        }


    }
}
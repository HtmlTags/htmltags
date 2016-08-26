namespace HtmlTags
{
    using Conventions.Elements;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement("display-tag", TagStructure = TagStructure.WithoutEndTag)]
    public class DisplayTagHelper : HtmlTagTagHelper
    {
        protected override string Category { get; } = ElementConstants.Display;
    }
}
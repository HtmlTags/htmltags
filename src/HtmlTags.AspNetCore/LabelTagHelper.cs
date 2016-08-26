namespace HtmlTags
{
    using Conventions.Elements;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement("label-tag", TagStructure = TagStructure.WithoutEndTag)]
    public class LabelTagHelper : HtmlTagTagHelper
    {
        protected override string Category { get; } = ElementConstants.Label;
    }
}
namespace HtmlTags
{
    using Conventions.Elements;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement("label-tag", Attributes = ForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class LabelTagHelper : HtmlTagTagHelper
    {
        protected override string Category { get; } = ElementConstants.Label;
    }
}
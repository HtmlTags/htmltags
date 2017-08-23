namespace HtmlTags
{
    using Conventions.Elements;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement("input-tag", Attributes = nameof(For), TagStructure = TagStructure.WithoutEndTag)]
    public class InputTagHelper : HtmlTagTagHelper
    {
        protected override string Category { get; } = ElementConstants.Editor;
    }
}
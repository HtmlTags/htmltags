namespace HtmlTags
{
    using Conventions.Elements;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement("validation-message-tag", Attributes = ForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class ValidationMessageTagHelper : HtmlTagTagHelper
    {
        protected override string Category { get; } = ElementConstants.ValidationMessage;
    }
}
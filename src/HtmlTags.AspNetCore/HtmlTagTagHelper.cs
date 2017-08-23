using System;

namespace HtmlTags
{
    using Conventions;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class HtmlTagTagHelper : TagHelper
    {
        public const string ForAttributeName = "for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        protected abstract string Category { get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (For == null)
                throw new InvalidOperationException(
                    "Missing or invalid 'for' attribute value. Specify a valid model expression for the 'for' attribute value.");

            var request = new ElementRequest(new ModelMetadataAccessor(For))
            {
                Model = For.Model
            };

            var library = ViewContext.HttpContext.RequestServices.GetService<HtmlConventionLibrary>();

            var tagGenerator = new TagGenerator(library.TagLibrary, new ActiveProfile(), t => ViewContext.HttpContext.RequestServices.GetService(t));

            var tag = tagGenerator.Build(request, Category);

            foreach (var attribute in output.Attributes)
            {
                tag.Attr(attribute.Name, attribute.Value);
            }

            output.TagName = null;
            output.PreElement.AppendHtml(tag);
        }
    }
}
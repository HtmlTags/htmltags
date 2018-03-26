using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements.Builders;

namespace HtmlTags
{
    public static class ModelStateTagExtensions
    {
        public static HtmlConventionRegistry ModelState(this HtmlConventionRegistry registry)
        {
            registry.ValidationMessages.Always.BuildBy<DefaultValidationMessageBuilder>();

            return registry;
        }
    }
}
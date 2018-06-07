using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;
using HtmlTags.Conventions.Elements.Builders;

namespace HtmlTags
{
    public static class ModelStateTagExtensions
    {
        public static HtmlConventionRegistry ModelState(this HtmlConventionRegistry registry)
            => registry.ModelStateBuilders().ModelStateNamingConvention();

        public static HtmlConventionRegistry ModelStateNamingConvention(this HtmlConventionRegistry registry)
        {
            registry.ValidationMessages.NamingConvention(new DotNotationElementNamingConvention());

            return registry;
        }

        public static HtmlConventionRegistry ModelStateBuilders(this HtmlConventionRegistry registry)
        {
            registry.ValidationMessages.Always.BuildBy<DefaultValidationMessageBuilder>();

            return registry;
        }
    }
}
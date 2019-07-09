namespace HtmlTags.Conventions
{
    using Elements;
    using Elements.Builders;

    public static class HtmlConventionRegistryExtensions
    {
        public static HtmlConventionRegistry Defaults(this HtmlConventionRegistry registry) =>
            registry
                .DefaultBuilders()
                .DefaultModifiers()
                .DefaultNamingConvention();

        public static HtmlConventionRegistry DefaultNamingConvention(this HtmlConventionRegistry registry)
        {
            registry.Editors.NamingConvention(new DotNotationElementNamingConvention());
            registry.Labels.NamingConvention(new DotNotationElementNamingConvention());

            return registry;
        }

        public static HtmlConventionRegistry DefaultModifiers(this HtmlConventionRegistry registry)
        {
            registry.Editors.Modifier<AddNameModifier>();

            registry.Editors.Modifier<AddIdModifier>();

            return registry;
        }

        public static HtmlConventionRegistry DefaultBuilders(this HtmlConventionRegistry registry)
        {
            registry.Editors.BuilderPolicy<CheckboxBuilder>();

            registry.Editors.Always.BuildBy<TextboxBuilder>();

            registry.Displays.Always.BuildBy<SpanDisplayBuilder>();

            registry.Labels.Always.BuildBy<DefaultLabelBuilder>();

            return registry;
        }
    }
}
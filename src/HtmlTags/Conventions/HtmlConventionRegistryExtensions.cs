namespace HtmlTags.Conventions
{
    using Elements;
    using Elements.Builders;

    public static class HtmlConventionRegistryExtensions
    {
        public static void Defaults(this HtmlConventionRegistry registry)
        {
            registry.Editors.BuilderPolicy<CheckboxBuilder>();

            registry.Editors.Always.BuildBy<TextboxBuilder>();

            registry.Editors.Modifier<AddNameModifier>();

            registry.Editors.Modifier<AddIdModifier>();

            registry.Editors.NamingConvention(new DotNotationElementNamingConvention());

            registry.Displays.Always.BuildBy<SpanDisplayBuilder>();

            registry.Labels.Always.BuildBy<DefaultLabelBuilder>();


        }
    }
}
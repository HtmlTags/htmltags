namespace HtmlTags.Conventions
{
    using Elements.Builders;

    public class DefaultHtmlConventions : HtmlConventionRegistry
    {
        public DefaultHtmlConventions()
        {
            Editors.BuilderPolicy<CheckboxBuilder>();

            Editors.Always.BuildBy<TextboxBuilder>();

            Editors.Modifier<AddNameModifier>();

            Displays.Always.BuildBy<SpanDisplayBuilder>();

            Labels.Always.BuildBy<DefaultLabelBuilder>();
        }
    }
}
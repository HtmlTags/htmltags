namespace HtmlTags.Conventions.Elements.Builders
{
    public class CheckboxBuilder : ElementTagBuilder
    {
        public override bool Matches(ElementRequest subject) => subject?.Accessor?.PropertyType == typeof (bool);

        public override HtmlTag Build(ElementRequest request) => new CheckboxTag(request?.RawValue?.As<bool>() ?? false);
    }
}
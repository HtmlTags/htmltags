namespace HtmlTags.Conventions.Elements.Builders
{
    using System.ComponentModel;

    [Description("Adds @name=[Accessor name] to any input elements to facilitate model binding")]
    public class AddNameModifier : IElementModifier
    {
        public bool Matches(ElementRequest token)
        {
            return true;
        }

        public void Modify(ElementRequest request)
        {
            var tag = request.CurrentTag;
            if (tag.IsInputElement() && !tag.HasAttr("name"))
            {
                tag.Attr("name", request.ElementId);
            }
        }
    }
}
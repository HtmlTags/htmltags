namespace HtmlTags.Conventions.Elements.Builders
{
    public class AddNameModifier : IElementModifier
    {
        public bool Matches(ElementRequest token) => true;

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
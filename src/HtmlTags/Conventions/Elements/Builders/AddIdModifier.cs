namespace HtmlTags.Conventions.Elements.Builders
{
    public class AddIdModifier : IElementModifier
    {
        public bool Matches(ElementRequest token) => true;

        public void Modify(ElementRequest request)
        {
            var tag = request.CurrentTag;
            if (tag.IsInputElement() && !tag.HasAttr("id"))
            {
                tag.Id(DefaultIdBuilder.Build(request));
            }
        }
    }
}
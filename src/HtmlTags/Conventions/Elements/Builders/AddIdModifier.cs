namespace HtmlTags.Conventions.Elements.Builders
{
    using System.Text.RegularExpressions;

    public class AddIdModifier : IElementModifier
    {
        private static readonly Regex IdRegex = new Regex(@"[\.\[\]]");

        public bool Matches(ElementRequest token) => true;

        public void Modify(ElementRequest request)
        {
            var tag = request.CurrentTag;
            if (tag.IsInputElement() && !tag.HasAttr("id"))
            {
                tag.Id(IdRegex.Replace(request.ElementId, "_"));
            }
        }
    }
}
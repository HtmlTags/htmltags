namespace HtmlTags.Conventions.Elements.Builders
{
    public class AddClassModifier : IElementModifier
    {
        private readonly string _className;

        public AddClassModifier(string className)
        {
            _className = className;
        }

        public bool Matches(ElementRequest token) => true;

        public void Modify(ElementRequest request) => request.CurrentTag.AddClass(_className);
    }
}
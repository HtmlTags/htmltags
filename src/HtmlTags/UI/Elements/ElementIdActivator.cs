namespace HtmlTags.UI.Elements
{
    public class ElementIdActivator : TagRequestActivator<ElementRequest>
    {
        private readonly IElementNamingConvention _naming;

        public ElementIdActivator(IElementNamingConvention naming)
        {
            _naming = naming;
        }

        public override void Activate(ElementRequest request)
        {
            request.ElementId = string.IsNullOrEmpty(request.ElementId)
                ? _naming.GetName(request.HolderType(), request.Accessor)
                : request.ElementId;
        }
    }
}
namespace HtmlTags.Conventions.Elements.Builders
{
    public class SpanDisplayBuilder : IElementBuilder
    {
        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("span").Text(request.StringValue()).Id(request.ElementId);
        }
    }
    
}
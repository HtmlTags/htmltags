namespace HtmlTags.Conventions.Elements.Builders
{
    using System.ComponentModel;

    [Description("Builds a <span>[accessor value]</span> element using IDisplayFormatter")]
    public class SpanDisplayBuilder : IElementBuilder
    {
        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("span").Text(request.StringValue()).Id(request.ElementId);
        }
    }
    
}
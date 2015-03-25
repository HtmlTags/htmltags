namespace HtmlTags.Conventions.Elements
{
    public interface IElementBuilder : ITagBuilder
    {
        
    }

    public interface IElementBuilderPolicy : ITagBuilderPolicy
    {
        
    }

    public abstract class ElementTagBuilder : TagBuilder, IElementBuilderPolicy, IElementBuilder
    {
        
    }
}
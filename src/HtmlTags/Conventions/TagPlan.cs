using System.Collections.Generic;

namespace HtmlTags.Conventions
{
    using Elements;

    public interface ITagPlan
    {
        HtmlTag Build(ElementRequest request);
    }

    public class TagPlan : ITagPlan
    {
        private readonly List<ITagModifier> _modifiers = new List<ITagModifier>();

        public TagPlan(ITagBuilder builder, IEnumerable<ITagModifier> modifiers, IElementNamingConvention elementNamingConvention)
        {
            Builder = builder;
            ElementNamingConvention = elementNamingConvention;

            _modifiers.AddRange(modifiers); // Important to force the enumerable to be executed no later than this point
        }

        public ITagBuilder Builder { get; }

        public IEnumerable<ITagModifier> Modifiers => _modifiers;

        public IElementNamingConvention ElementNamingConvention { get; }

        public HtmlTag Build(ElementRequest request)
        {
            request.ElementId = string.IsNullOrEmpty(request.ElementId)
                ? ElementNamingConvention.GetName(request.HolderType(), request.Accessor)
                : request.ElementId;

            var tag = Builder.Build(request);
            request.ReplaceTag(tag);

            _modifiers.Each(m => m.Modify(request));

            return request.CurrentTag;
        }
    }
}
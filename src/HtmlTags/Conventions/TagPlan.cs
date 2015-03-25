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
        private readonly ITagBuilder _builder;
        private readonly IElementNamingConvention _elementNamingConvention;
        private readonly List<ITagModifier> _modifiers = new List<ITagModifier>();

        public TagPlan(ITagBuilder builder, IEnumerable<ITagModifier> modifiers, IElementNamingConvention elementNamingConvention)
        {
            _builder = builder;
            _elementNamingConvention = elementNamingConvention;

            _modifiers.AddRange(modifiers); // Important to force the enumerable to be executed no later than this point
        }

        public ITagBuilder Builder
        {
            get { return _builder; }
        }

        public IEnumerable<ITagModifier> Modifiers
        {
            get { return _modifiers; }
        }

        public IElementNamingConvention ElementNamingConvention
        {
            get { return _elementNamingConvention; }
        }

        public HtmlTag Build(ElementRequest request)
        {
            request.ElementId = string.IsNullOrEmpty(request.ElementId)
                ? ElementNamingConvention.GetName(request.HolderType(), request.Accessor)
                : request.ElementId;

            var tag = _builder.Build(request);
            request.ReplaceTag(tag);

            _modifiers.Each(m => m.Modify(request));

            return request.CurrentTag;
        }
    }
}
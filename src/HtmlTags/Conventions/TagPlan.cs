using System.Collections.Generic;

namespace HtmlTags.Conventions
{
    public interface ITagPlan<T> where T : TagRequest
    {
        HtmlTag Build(T request);
    }

    public class TagPlan<T> : ITagPlan<T> where T : TagRequest
    {
        private readonly ITagBuilder<T> _builder;
        private readonly List<ITagModifier<T>> _modifiers = new List<ITagModifier<T>>();

        public TagPlan(ITagBuilder<T> builder, IEnumerable<ITagModifier<T>> modifiers)
        {
            _builder = builder;
            
            _modifiers.AddRange(modifiers); // Important to force the enumerable to be executed no later than this point
        }

        public ITagBuilder<T> Builder
        {
            get { return _builder; }
        }

        public IEnumerable<ITagModifier<T>> Modifiers
        {
            get { return _modifiers; }
        }

        public HtmlTag Build(T request)
        {
            var tag = _builder.Build(request);
            request.ReplaceTag(tag);

            _modifiers.Each(m => m.Modify(request));

            return request.CurrentTag;
        }
    }
}
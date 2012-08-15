using System;
using System.Collections.Generic;

namespace HtmlTags.Conventions
{
    // Tested through the test for TagCategory and TagLibrary
    public class BuilderSet<T> : ITagBuildingExpression<T> where T : TagRequest
    {
        private readonly IList<ITagBuilder<T>> _builders = new List<ITagBuilder<T>>();
        private readonly IList<ITagModifier<T>> _modifiers = new List<ITagModifier<T>>();

        public IEnumerable<ITagBuilder<T>> Builders
        {
            get { return _builders; }
        }

        public IEnumerable<ITagModifier<T>> Modifiers
        {
            get { return _modifiers; }
        }

        public void Add(ITagBuilder<T> builder)
        {
            _builders.Add(builder);
        }

        public void Add(ITagModifier<T> modifier)
        {
            _modifiers.Add(modifier);
        }

        public CategoryExpression<T> Always
        {
            get { return new CategoryExpression<T>(this, x => true); }
        }

        public CategoryExpression<T> If(Func<T, bool> matches)
        {
            return new CategoryExpression<T>(this, matches);
        }
    }
}
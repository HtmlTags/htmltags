using System;
using System.Collections.Generic;

namespace HtmlTags.Conventions
{
    // Tested through the test for TagCategory and TagLibrary
    public class BuilderSet<T> : ITagBuildingExpression<T> where T : TagRequest
    {
        private readonly List<ITagBuilderPolicy<T>> _policies = new List<ITagBuilderPolicy<T>>();
        private readonly List<ITagModifier<T>> _modifiers = new List<ITagModifier<T>>();

        public IEnumerable<ITagBuilderPolicy<T>> Policies
        {
            get { return _policies; }
        }

        public IEnumerable<ITagModifier<T>> Modifiers
        {
            get { return _modifiers; }
        }

        public void Add(Func<T, bool> filter, ITagBuilder<T> builder)
        {
            _policies.Add(new ConditionalTagBuilderPolicy<T>(filter, builder));
        }

        public void Add(ITagBuilderPolicy<T> policy)
        {
            _policies.Add(policy);
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

        public void Import(BuilderSet<T> other)
        {
            _policies.AddRange(other._policies);
            _modifiers.AddRange(other._modifiers);
        }
    }
}
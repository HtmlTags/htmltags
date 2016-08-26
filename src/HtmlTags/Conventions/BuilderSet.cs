using System;
using System.Collections.Generic;

namespace HtmlTags.Conventions
{
    using Elements;

    // Tested through the test for TagCategory and TagLibrary
    public class BuilderSet : ITagBuildingExpression
    {
        private readonly List<ITagBuilderPolicy> _policies = new List<ITagBuilderPolicy>();
        private readonly List<ITagModifier> _modifiers = new List<ITagModifier>();
        private IElementNamingConvention _elementNamingConvention;

        public BuilderSet()
        {
            _elementNamingConvention = new DefaultElementNamingConvention();
        }

        public IEnumerable<ITagBuilderPolicy> Policies => _policies;

        public IEnumerable<ITagModifier> Modifiers => _modifiers;

        public IElementNamingConvention ElementNamingConvention => _elementNamingConvention;

        public void Add(Func<ElementRequest, bool> filter, ITagBuilder builder) 
            => _policies.Add(new ConditionalTagBuilderPolicy(filter, builder));

        public void Add(ITagBuilderPolicy policy) => _policies.Add(policy);

        public void Add(ITagModifier modifier) 
            => _modifiers.Add(modifier);

        public void NamingConvention(IElementNamingConvention elementNamingConvention) 
            => _elementNamingConvention = elementNamingConvention;

        public CategoryExpression Always => new CategoryExpression(this, x => true);

        public CategoryExpression If(Func<ElementRequest, bool> matches) => new CategoryExpression(this, matches);

        public void Import(BuilderSet other)
        {
            _policies.AddRange(other._policies);
            _modifiers.AddRange(other._modifiers);
            _elementNamingConvention = other._elementNamingConvention;
        }

        public void InsertFirst(ITagBuilderPolicy policy) => _policies.Insert(0, policy);
    }
}
using System;

namespace HtmlTags.Conventions
{
    // Tested through the tests for TagCategory and TagLibrary
    public class CategoryExpression
    {
        private readonly BuilderSet _parent;
        private readonly Func<ElementRequest, bool> _matcher;

        public CategoryExpression(BuilderSet parent, Func<ElementRequest, bool> matcher)
        {
            _parent = parent;
            _matcher = matcher;
        }

        public void Modify(Action<ElementRequest> modify) => _parent.Add(new LambdaTagModifier(_matcher, modify));

        public void Build(Func<ElementRequest, HtmlTag> build) => _parent.Add(new ConditionalTagBuilderPolicy(_matcher, build));
    }
}
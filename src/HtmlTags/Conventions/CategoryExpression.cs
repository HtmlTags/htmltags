using System;

namespace HtmlTags.Conventions
{
    // Tested through the tests for TagCategory and TagLibrary
    public class CategoryExpression<T> where T : TagRequest
    {
        private readonly BuilderSet<T> _parent;
        private readonly Func<T, bool> _matcher;

        public CategoryExpression(BuilderSet<T> parent, Func<T, bool> matcher)
        {
            _parent = parent;
            _matcher = matcher;
        }

        public void Modify(Action<T> modify)
        {
            _parent.Add(new LambdaTagModifier<T>(_matcher, modify));
        }

        public void Build(Func<T, HtmlTag> build)
        {
            _parent.Add(new LambdaTagBuilder<T>(_matcher, build));
        }
    }
}
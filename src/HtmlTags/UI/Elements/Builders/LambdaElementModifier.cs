namespace HtmlTags.UI.Elements.Builders
{
    using System;
    using Conventions;

    // Tested through HtmlConventionRegistry
    public class LambdaElementModifier : LambdaTagModifier<ElementRequest>, IElementModifier
    {
        public LambdaElementModifier(Func<ElementRequest, bool> matcher, Action<ElementRequest> modify)
            : base(matcher, modify)
        {
        }

        public LambdaElementModifier(Action<ElementRequest> modify) : base(modify)
        {
        }

        public string ConditionDescription { get; set; }
        public string ModifierDescription { get; set; }
    }
}
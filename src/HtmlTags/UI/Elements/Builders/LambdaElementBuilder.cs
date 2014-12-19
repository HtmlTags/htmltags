namespace HtmlTags.UI.Elements.Builders
{
    using System;
    using Conventions;

    //Tested through HtmlConventionRegistry tests
    public class LambdaElementBuilder : TagBuilder<ElementRequest>
    {
        private readonly Func<ElementRequest, bool> _matcher;
        private readonly Func<ElementRequest, HtmlTag> _build;

        public LambdaElementBuilder(Func<ElementRequest, HtmlTag> build) : this(x => true, build)
        {
            ConditionDescription = "Always";
        }

        public LambdaElementBuilder(Func<ElementRequest, bool> matcher, Func<ElementRequest, HtmlTag> build)
        {
            _matcher = matcher;
            _build = build;
        }

        public string ConditionDescription { get; set; }
        public string BuilderDescription { get; set; }

        public override bool Matches(ElementRequest subject)
        {
            return _matcher(subject);
        }

        public override HtmlTag Build(ElementRequest request)
        {
            return _build(request);
        }
    }
}
namespace HtmlTags.Conventions.Elements.Builders
{
    using System;

    //Tested through HtmlConventionRegistry tests
    public class LambdaElementBuilder : TagBuilder
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

        public override bool Matches(ElementRequest subject) => _matcher(subject);

        public override HtmlTag Build(ElementRequest request) => _build(request);
    }
}
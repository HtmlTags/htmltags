namespace HtmlTags.Conventions.Elements
{
    using System;

    public class AccessorOverrideBuilderPolicy : IElementBuilderPolicy
    {
        public bool Matches(ElementRequest subject)
        {
            var overrides = subject.Get<HtmlTags.Reflection.AccessorRules>();


            throw new NotImplementedException();
        }

        public ITagBuilder BuilderFor(ElementRequest subject)
        {
            throw new System.NotImplementedException();
        }
    }
}
using System;
using HtmlTags.Reflection;
using HtmlTags.Conventions;
using System.Linq;

namespace HtmlTags.UI.Elements
{
    public class AccessorOverrideElementBuilderPolicy : IElementBuilderPolicy
    {
        private readonly AccessorRules _rules;
        private readonly string _category;
        private readonly string _profile;

        public AccessorOverrideElementBuilderPolicy(AccessorRules rules, string category, string profile)
        {
            _rules = rules;
            _category = category;
            _profile = profile;
        }

        public bool Matches(ElementRequest subject)
        {
            return _rules.AllRulesFor<IElementTagOverride>(subject.Accessor).Any(x => x.Category == _category && x.Profile == _profile);
        }

        public ITagBuilder<ElementRequest> BuilderFor(ElementRequest subject)
        {
            return
                _rules.AllRulesFor<IElementTagOverride>(subject.Accessor)
                      .First(x => x.Category == _category && x.Profile == _profile)
                      .Builder();
        }
    }
}
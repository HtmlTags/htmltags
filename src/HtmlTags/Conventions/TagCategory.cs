using System;
using System.Collections.Generic;
using System.Linq;

namespace HtmlTags.Conventions
{
    public class TagCategory : ITagBuildingExpression
    {
        private readonly BuilderSet _defaults = new BuilderSet();
        private readonly Cache<TagSubject, TagPlan> _plans = new Cache<TagSubject, TagPlan>();

        private readonly Cache<string, BuilderSet> _profiles =
            new Cache<string, BuilderSet>(name => new BuilderSet());

        public TagCategory()
        {
            _profiles[TagConstants.Default] = _defaults;
            _plans.OnMissing = buildPlan;
        }

        public BuilderSet Defaults
        {
            get { return _defaults; }
        }

        public BuilderSet Profile(string name)
        {
            return _profiles[name];
        }

        public TagPlan PlanFor(ElementRequest request, string profile = null)
        {
            var subject = new TagSubject(profile, request);
            return _plans[subject];
        }

        private TagPlan buildPlan(TagSubject subject)
        {
            var sets = setsFor(subject.Profile).ToList();
            var policy = sets.SelectMany(x => x.Policies).FirstOrDefault(x => x.Matches(subject.Subject));
            if (policy == null)
            {
                throw new ArgumentOutOfRangeException("Unable to select a TagBuilderPolicy for subject " + subject);
            }

            var builder = policy.BuilderFor(subject.Subject);

            var modifiers = sets.SelectMany(x => x.Modifiers).Where(x => x.Matches(subject.Subject));

            var elementNamingConvention = sets.Select(x => x.ElementNamingConvention).FirstOrDefault();

            return new TagPlan(builder, modifiers, elementNamingConvention);
        }

        private IEnumerable<BuilderSet> setsFor(string profile)
        {
            if (!string.IsNullOrEmpty(profile) && profile != TagConstants.Default)
            {
                yield return _profiles[profile];
            }

            yield return _defaults;
        }

        public void ClearPlans()
        {
            _plans.ClearAll();
        }

        public void Add(Func<ElementRequest, bool> filter, ITagBuilder builder)
        {
            Add(new ConditionalTagBuilderPolicy(filter, builder));
        }

        public void Add(ITagBuilderPolicy policy)
        {
            _profiles[TagConstants.Default].Add(policy);
        }

        public void Add(ITagModifier modifier)
        {
            _profiles[TagConstants.Default].Add(modifier);
        }


        public CategoryExpression Always
        {
            get { return _defaults.Always; }
        }

        public CategoryExpression If(Func<ElementRequest, bool> matches)
        {
            return _defaults.If(matches);
        }

        public ITagBuildingExpression ForProfile(string profile)
        {
            return _profiles[profile];
        }

        public void Import(TagCategory other)
        {
            _defaults.Import(other._defaults);

            var keys = _profiles.GetKeys().Union(other._profiles.GetKeys())
                .Where(x => x != TagConstants.Default)
                .Distinct();

            keys.Each(key => _profiles[key].Import(other._profiles[key]));
        }
    }
}
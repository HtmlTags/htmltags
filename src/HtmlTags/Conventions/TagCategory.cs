using System;
using System.Collections.Generic;
using System.Linq;

namespace HtmlTags.Conventions
{
    public class TagCategory<T> : ITagBuildingExpression<T> where T : TagRequest
    {
        private readonly BuilderSet<T> _defaults = new BuilderSet<T>();
        private readonly Cache<TagSubject<T>, TagPlan<T>> _plans = new Cache<TagSubject<T>, TagPlan<T>>();

        private readonly Cache<string, BuilderSet<T>> _profiles =
            new Cache<string, BuilderSet<T>>(name => new BuilderSet<T>());

        public TagCategory()
        {
            _profiles[TagConstants.Default] = _defaults;
            _plans.OnMissing = buildPlan;
        }

        public BuilderSet<T> Defaults
        {
            get { return _defaults; }
        }

        public BuilderSet<T> Profile(string name)
        {
            return _profiles[name];
        }

        public TagPlan<T> PlanFor(T request, string profile = null)
        {
            var subject = new TagSubject<T>(profile, request);
            return _plans[subject];
        }

        private TagPlan<T> buildPlan(TagSubject<T> subject)
        {
            var sets = setsFor(subject.Profile);
            var builder = sets.SelectMany(x => x.Builders).FirstOrDefault(x => x.Matches(subject.Subject));

            if (builder == null)
            {
                throw new ArgumentOutOfRangeException("Unable to select a TagBuilder for subject " + subject);
            }

            var modifiers = sets.SelectMany(x => x.Modifiers).Where(x => x.Matches(subject.Subject));

            return new TagPlan<T>(builder, modifiers);
        }

        private IEnumerable<BuilderSet<T>> setsFor(string profile)
        {
            if (!string.IsNullOrEmpty(profile))
            {
                yield return _profiles[profile];
            }

            yield return _defaults;
        }

        public void ClearPlans()
        {
            _plans.ClearAll();
        }

        public void Add(ITagBuilder<T> builder)
        {
            _profiles[TagConstants.Default].Add(builder);
        }

        public void Add(ITagModifier<T> modifier)
        {
            _profiles[TagConstants.Default].Add(modifier);
        }


        public CategoryExpression<T> Always
        {
            get { return _defaults.Always; }
        }

        public CategoryExpression<T> If(Func<T, bool> matches)
        {
            return _defaults.If(matches);
        }

        public ITagBuildingExpression<T> ForProfile(string profile)
        {
            return _profiles[profile];
        }

        public void Import(TagCategory<T> other)
        {
            _defaults.Import(other._defaults);

            var keys = _profiles.GetKeys().Union(other._profiles.GetKeys())
                .Where(x => x != TagConstants.Default)
                .Distinct();

            keys.Each(key => _profiles[key].Import(other._profiles[key]));
        }
    }
}
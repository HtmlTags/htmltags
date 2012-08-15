using System;

namespace HtmlTags.Conventions
{
    public interface ITagLibrary<T> where T : TagRequest
    {
        ITagPlan<T> PlanFor(T subject, string profile = null, string category = null);
    }

    public class TagLibrary<T> : ITagBuildingExpression<T>, ITagLibrary<T> where T : TagRequest
    {
        private readonly Cache<string, TagCategory<T>> _categories =
            new Cache<string, TagCategory<T>>(name => new TagCategory<T>());

        public ITagPlan<T> PlanFor(T subject, string profile = null, string category = null)
        {
            profile = profile ?? TagConstants.Default;
            category = category ?? TagConstants.Default;

            return _categories[category].PlanFor(subject, profile);
        }

        public CategoryExpression<T> Always
        {
            get { return _categories[TagConstants.Default].Always; }
        }

        public CategoryExpression<T> If(Func<T, bool> matches)
        {
            return _categories[TagConstants.Default].If(matches);
        }

        public TagCategory<T> ForCategory(string category)
        {
            return _categories[category];
        }

        public void Add(ITagBuilder<T> builder)
        {
            _categories[TagConstants.Default].Add(builder);
        }

        public void Add(ITagModifier<T> modifier)
        {
            _categories[TagConstants.Default].Add(modifier);
        }

        public ITagBuildingExpression<T> ForProfile(string profile)
        {
            return _categories[TagConstants.Default].ForProfile(profile);
        }
    }
}
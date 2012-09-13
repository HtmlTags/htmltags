using System;
using System.Linq;

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

        /// <summary>
        /// The modifiers and builders for a category of conventions 
        /// </summary>
        /// <param name="category">Example:  "Label", "Editor", "Display"</param>
        /// <returns></returns>
        public TagCategory<T> Category(string category)
        {
            return _categories[category];
        }

        public BuilderSet<T> BuilderSetFor(string category = null, string profile = null)
        {
            profile = profile ?? TagConstants.Default;
            category = category ?? TagConstants.Default;

            return _categories[category].Profile(profile);
        }

        /// <summary>
        /// Adds a builder to the default category and profile
        /// </summary>
        /// <param name="builder"></param>
        public void Add(ITagBuilder<T> builder)
        {
            Default.Add(builder);
        }

        /// <summary>
        /// Adds a modifier to the default category and profile
        /// </summary>
        /// <param name="modifier"></param>
        public void Add(ITagModifier<T> modifier)
        {
            Default.Add(modifier);
        }

        /// <summary>
        /// Access to the default category
        /// </summary>
        public TagCategory<T> Default
        {
            get
            {
                return _categories[TagConstants.Default];
            }
        }

        /// <summary>
        /// Add builders and modifiers by profile
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public ITagBuildingExpression<T> ForProfile(string profile)
        {
            return _categories[TagConstants.Default].ForProfile(profile);
        }

        public void Import(TagLibrary<T> other)
        {
            var keys = _categories.GetKeys().Union(other._categories.GetKeys()).Distinct();

            keys.Each(key => _categories[key].Import(other._categories[key]));
        }
    }
}
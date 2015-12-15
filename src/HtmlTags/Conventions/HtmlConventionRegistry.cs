namespace HtmlTags.Conventions
{
    using System;
    using Elements;
    using Elements.Builders;

    public class HtmlConventionRegistry : ProfileExpression
    {
        public HtmlConventionRegistry() : this(new HtmlConventionLibrary())
        {
        }

        private HtmlConventionRegistry(HtmlConventionLibrary library) : base(library, TagConstants.Default)
        {
        }

        public void Profile(string profileName, Action<ProfileExpression> configure)
        {
            var expression = new ProfileExpression(Library, profileName);
            configure(expression);
        }
    }

    public class ElementActionExpression
    {
        private readonly BuilderSet _set;
        private readonly Func<ElementRequest, bool> _filter;
        private readonly string _filterDescription;

        public ElementActionExpression(BuilderSet set, Func<ElementRequest, bool> filter,
            string filterDescription)
        {
            _set = set;
            _filter = filter;
            _filterDescription = filterDescription.IsNotEmpty() ? filterDescription : "User Defined";
        }

        public void BuildBy(IElementBuilder builder)
        {
            var conditionalBuilder = new ConditionalElementBuilder(_filter, builder)
            {
                ConditionDescription = _filterDescription
            };

            _set.Add(conditionalBuilder);
        }

        public void BuildBy<T>() where T : IElementBuilder, new()
        {
            BuildBy(new T());
        }

        public void BuildBy(Func<ElementRequest, HtmlTag> tagBuilder, string description = null)
        {
            var builder = new LambdaElementBuilder(_filter, tagBuilder)
            {
                ConditionDescription = _filterDescription,
                BuilderDescription = description ?? "User Defined"
            };

            _set.Add(builder);
        }

        public void ModifyWith(IElementModifier modifier)
        {
            var conditionalModifier = new ConditionalElementModifier(_filter, modifier)
            {
                ConditionDescription = _filterDescription
            };

            _set.Add(conditionalModifier);
        }

        public void ModifyWith<T>() where T : IElementModifier, new()
        {
            ModifyWith(new T());
        }

        public void ModifyWith(Action<ElementRequest> modification, string description = null)
        {
            var modifier = new LambdaElementModifier(_filter, modification)
            {
                ConditionDescription = _filterDescription,
                ModifierDescription = description ?? "User Defined"
            };

            _set.Add(modifier);
        }

        public void Attr(string attName, object value)
        {
            ModifyWith(req => req.CurrentTag.Attr(attName, value), string.Format("Set @{0} = '{1}'", new[] {attName, value}));
        }

        public void AddClass(string className)
        {
            ModifyWith(req => req.CurrentTag.AddClass(className), string.Format("Add class '{0}'", new[] {className}));
        }
    }
}
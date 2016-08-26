namespace HtmlTags.Conventions
{
    using System;
    using Elements;
    using Reflection;

    public class ElementCategoryExpression
    {
        private readonly BuilderSet _set;

        public ElementCategoryExpression(BuilderSet set)
        {
            _set = set;
        }

        public void Add(Func<ElementRequest, bool> filter, IElementBuilder builder) => _set.Add(filter, builder);

        public void Add(IElementBuilderPolicy policy) => _set.Add(policy);

        public void Add(IElementModifier modifier) => _set.Add(modifier);

        public void BuilderPolicy<T>() where T : IElementBuilderPolicy, new() => Add(new T());

        public void Modifier<T>() where T : IElementModifier, new() => Add(new T());

        public void NamingConvention(IElementNamingConvention elementNamingConvention)
            => _set.NamingConvention(elementNamingConvention);

        public ElementActionExpression Always => new ElementActionExpression(_set, req => true, "Always");

        public ElementActionExpression If(Func<ElementRequest, bool> matches, string description = null) 
            => new ElementActionExpression(_set, matches, description);

        public ElementActionExpression IfPropertyIs<T>() => If(req => req.Accessor.PropertyType == typeof(T),
            $"Property type is {typeof (T).Name}");

        public ElementActionExpression IfPropertyTypeIs(Func<Type, bool> matches, string description = null) 
            => If(def => matches(def.Accessor.PropertyType), description);

        public ElementActionExpression IfPropertyHasAttribute<T>() where T : Attribute 
            => If(req => req.Accessor.HasAttribute<T>(), $"Accessor has attribute [{typeof(T).Name}]");

        public void AddClassForAttribute<T>(string className) where T : Attribute 
            => IfPropertyHasAttribute<T>().AddClass(className);

        public void ModifyForAttribute<T>(Action<HtmlTag, T> modification, string description = null) where T : Attribute
        {
            IfPropertyHasAttribute<T>().ModifyWith(req =>
            {
                var att = req.Accessor.GetAttribute<T>();
                modification(req.CurrentTag, att);
            }, description);
        }

        public void ModifyForAttribute<T>(Action<HtmlTag> modification, string description = null) where T : Attribute 
            => ModifyForAttribute<T>((tag, att) => modification(tag), description);
    }
}
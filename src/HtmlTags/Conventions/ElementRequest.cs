namespace HtmlTags.Conventions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Elements;
    using Formatting;
    using Reflection;

    public class ElementRequest
    {
        private bool _hasFetched;
        private object _rawValue;
        private Func<Type, object> _services;

        public static ElementRequest For(object model, PropertyInfo property)
        {
            return new ElementRequest(new SingleProperty(property))
            {
                Model = model
            };
        }

        public static ElementRequest For<T>(Expression<Func<T, object>> expression) => new ElementRequest(expression.ToAccessor());

        public static ElementRequest For<T>(T model, Expression<Func<T, object>> expression)
        {
            return new ElementRequest(expression.ToAccessor())
            {
                Model = model
            };
        }

        public ElementRequest(Accessor accessor)
        {
            Accessor = accessor;
        }

        public object RawValue
        {
            get
            {
                if (!_hasFetched)
                {
                    _rawValue = Model == null ? null : Accessor.GetValue(Model);
                    _hasFetched = true;
                }

                return _rawValue;
            }
        }

        public string ElementId { get; set; }
        public object Model { get; set; }
        public Accessor Accessor { get; }
        public HtmlTag OriginalTag { get; private set; }
        public HtmlTag CurrentTag { get; private set; }

        public void WrapWith(HtmlTag tag)
        {
            CurrentTag.WrapWith(tag);
            ReplaceTag(tag);
        }

        public void ReplaceTag(HtmlTag tag)
        {
            if (OriginalTag == null)
            {
                OriginalTag = tag;
            }

            CurrentTag = tag;
        }

        public AccessorDef ToAccessorDef() => new AccessorDef(Accessor, HolderType());


        public Type HolderType() => Model == null ? Accessor.DeclaringType : Model?.GetType();

        public T Get<T>() => (T)_services(typeof(T));

        // virtual for mocking
        public virtual HtmlTag BuildForCategory(string category, string profile = null) => Get<ITagGenerator>().Build(this, category, profile);

        public T Value<T>() => (T) RawValue;

        public string StringValue() => new DisplayFormatter(_services).GetDisplay(new GetStringRequest(Accessor, RawValue, _services, null, null));

        public bool ValueIsEmpty() => RawValue == null || string.Empty.Equals(RawValue);

        public void ForValue<T>(Action<T> action)
        {
            if (ValueIsEmpty()) return;

            action((T) RawValue);
        }

        public void Attach(Func<Type, object> locator) => _services = locator;

        public ElementRequest ToToken() => new ElementRequest(Accessor);
    }
}
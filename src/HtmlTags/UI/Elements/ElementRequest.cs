namespace HtmlTags.UI.Elements
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Conventions;
    using Formatting;
    using Reflection;

    public class ElementRequest : TagRequest, IServiceLocatorAware
    {
        private bool _hasFetched;
        private object _rawValue;
        private IServiceLocator _services;

        public static ElementRequest For(object model, PropertyInfo property)
        {
            return new ElementRequest(new SingleProperty(property))
            {
                Model = model
            };
        }

        public static ElementRequest For<T>(Expression<Func<T, object>> expression)
        {
            return new ElementRequest(expression.ToAccessor());
        }

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
        public Accessor Accessor { get; private set; }

        public AccessorDef ToAccessorDef()
        {
            return new AccessorDef
            {
                Accessor = Accessor,
                ModelType = HolderType()
            };
        }


        public Type HolderType()
        {
            if (Model == null)
            {
                return Accessor.DeclaringType;
            }

            return Model == null ? null : Model.GetType();
        }

        public T Get<T>()
        {
            return _services.GetInstance<T>();
        }

        // virtual for mocking
        public virtual HtmlTag BuildForCategory(string category, string profile = null)
        {
            return Get<ITagGenerator<ElementRequest>>().Build(this, category, profile);
        }

        public T Value<T>()
        {
            return (T) RawValue;
        }

        public string StringValue()
        {
            return new DisplayFormatter(_services).GetDisplay(new GetStringRequest(Accessor, RawValue, _services));
        }

        public bool ValueIsEmpty()
        {
            return RawValue == null || string.Empty.Equals(RawValue);
        }

        public void ForValue<T>(Action<T> action)
        {
            if (ValueIsEmpty()) return;

            action((T) RawValue);
        }

        public void Attach(IServiceLocator locator)
        {
            _services = locator;
        }

        public override object ToToken()
        {
            return new ElementRequest(Accessor);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HtmlTags.Reflection
{
    public class PropertyChain : Accessor
    {
        private readonly IValueGetter[] _chain;
        private readonly IValueGetter[] _valueGetters;


        public PropertyChain(IValueGetter[] valueGetters)
        {
            _chain = new IValueGetter[valueGetters.Length - 1];
            for (int i = 0; i < _chain.Length; i++)
            {
                _chain[i] = valueGetters[i];
            }

            _valueGetters = valueGetters;
        }

        public IValueGetter[] ValueGetters => _valueGetters;


        public void SetValue(object target, object propertyValue)
        {
            target = FindInnerMostTarget(target);
            if (target == null)
            {
                return;
            }

            SetValueOnInnerObject(target, propertyValue);
        }

        public object GetValue(object target)
        {
            target = FindInnerMostTarget(target);

            return target == null ? null : _valueGetters.Last().GetValue(target);
        }

        public Type OwnerType
        {
            get
            {
                // Check if we're an indexer here
                var last = _valueGetters.Last();
                if (last is MethodValueGetter || last is IndexerValueGetter)
                {
                    var nextUp = _chain.Reverse().Skip(1).FirstOrDefault() as PropertyValueGetter;
                    if (nextUp != null)
                    {
                        return nextUp.PropertyInfo.PropertyType;
                    }
                }

                var propertyGetter = _chain.Last() as PropertyValueGetter;

                return propertyGetter?.PropertyInfo.PropertyType ?? InnerProperty?.DeclaringType;
            }
        }

        public string FieldName
        {
            get { 
                var last = _valueGetters.Last();
                if (last is PropertyValueGetter) return last.Name;

                var previous = _valueGetters[_valueGetters.Length - 2];
                return previous.Name + last.Name;
            }
        }

        public Type PropertyType => _valueGetters.Last().ValueType;

        public PropertyInfo InnerProperty => (_valueGetters.Last() as PropertyValueGetter)?.PropertyInfo;

        public Type DeclaringType => _chain[0].DeclaringType;

        public Accessor GetChildAccessor<T>(Expression<Func<T, object>> expression)
        {
            var accessor = expression.ToAccessor();
            var allGetters = Getters().Union(accessor.Getters()).ToArray();
            return new PropertyChain(allGetters);
        }

        public string[] PropertyNames => _valueGetters.Select(x => x.Name).ToArray();


        public Expression<Func<T, object>> ToExpression<T>()
        {
            ParameterExpression parameter = Expression.Parameter(typeof (T), "x");
            Expression body = parameter;

            _valueGetters.Each(getter => { body = getter.ChainExpression(body); });

            Type delegateType = typeof (Func<,>).MakeGenericType(typeof (T), typeof (object));
            return (Expression<Func<T, object>>) Expression.Lambda(delegateType, body, parameter);
        }

        public Accessor Prepend(PropertyInfo property)
        {
            var list = new List<IValueGetter>
            {
                new PropertyValueGetter(property)
            };
            list.AddRange(_valueGetters);

            return new PropertyChain(list.ToArray());
        }

        public IEnumerable<IValueGetter> Getters() => _valueGetters;


        /// <summary>
        ///     Concatenated names of all the properties in the chain.
        ///     Case.Site.Name == "CaseSiteName"
        /// </summary>
        public string Name => _valueGetters.Select(x => x.Name).Join("");

        protected virtual void SetValueOnInnerObject(object target, object propertyValue) 
            => _valueGetters.Last().SetValue(target, propertyValue);


        protected object FindInnerMostTarget(object target)
        {
            foreach (IValueGetter info in _chain)
            {
                target = info.GetValue(target);
                if (target == null)
                {
                    return null;
                }
            }

            return target;
        }


        public override string ToString() => _chain.First().DeclaringType.FullName + _chain.Select(x => x.Name).Join(".");

        public bool Equals(PropertyChain other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return _valueGetters.SequenceEqual(other._valueGetters);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (PropertyChain)) return false;
            return Equals((PropertyChain) obj);
        }

        public override int GetHashCode() => _chain?.GetHashCode() ?? 0;
    }
}
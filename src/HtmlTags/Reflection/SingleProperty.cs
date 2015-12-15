using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace HtmlTags.Reflection
{
    public class SingleProperty : Accessor
    {
        private readonly Type _ownerType;

        public SingleProperty(PropertyInfo property)
        {
            InnerProperty = property;
        }

        public SingleProperty(PropertyInfo property, Type ownerType)
        {
            InnerProperty = property;
            _ownerType = ownerType;
        }


        public string FieldName => InnerProperty.Name;

        public Type PropertyType => InnerProperty.PropertyType;

        public Type DeclaringType => InnerProperty.DeclaringType;


        public PropertyInfo InnerProperty { get; }

        public Accessor GetChildAccessor<T>(Expression<Func<T, object>> expression)
        {
            PropertyInfo property = ReflectionHelper.GetProperty(expression);
            return new PropertyChain(new[] {new PropertyValueGetter(InnerProperty), new PropertyValueGetter(property)});
        }

        public string[] PropertyNames => new[] { InnerProperty.Name };

        public Expression<Func<T, object>> ToExpression<T>()
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression body = Expression.Property(parameter, InnerProperty);
            if (InnerProperty.PropertyType.GetTypeInfo().IsValueType)
            {
                body = Expression.Convert(body, typeof (object));
            }


            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), typeof(object));
            return (Expression<Func<T, object>>) Expression.Lambda(delegateType, body, parameter);
        }

        public Accessor Prepend(PropertyInfo property)
        {
            return
                new PropertyChain(new IValueGetter[]
                                  {new PropertyValueGetter(property), new PropertyValueGetter(InnerProperty)});
        }

        public IEnumerable<IValueGetter> Getters()
        {
            yield return new PropertyValueGetter(InnerProperty);
        }

        public string Name => InnerProperty.Name;

        public virtual void SetValue(object target, object propertyValue)
        {
            if (InnerProperty.CanWrite)
            {
                InnerProperty.SetValue(target, propertyValue, null);
            }
        }

        public object GetValue(object target) => InnerProperty.GetValue(target, null);

        public Type OwnerType => _ownerType ?? DeclaringType;


        public static SingleProperty Build<T>(Expression<Func<T, object>> expression)
        {
            PropertyInfo property = ReflectionHelper.GetProperty(expression);
            return new SingleProperty(property);
        }

        public static SingleProperty Build<T>(string propertyName)
        {
            PropertyInfo property = typeof (T).GetProperty(propertyName);
            return new SingleProperty(property);
        }

        public bool Equals(SingleProperty other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return InnerProperty.PropertyMatches(other.InnerProperty);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SingleProperty)) return false;
            return Equals((SingleProperty) obj);
        }

        public override int GetHashCode() => (InnerProperty != null ? (InnerProperty.DeclaringType?.FullName + "." + InnerProperty.Name).GetHashCode() : 0);
    }
}
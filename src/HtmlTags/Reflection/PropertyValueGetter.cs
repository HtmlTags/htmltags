using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HtmlTags.Reflection
{
    public class PropertyValueGetter : IValueGetter
    {
        public PropertyValueGetter(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo { get; }

        public object GetValue(object target) => PropertyInfo.GetValue(target, null);

        public string Name => PropertyInfo.Name;

        public Type DeclaringType => PropertyInfo.DeclaringType;

        public Type ValueType => PropertyInfo.PropertyType;

        public Expression ChainExpression(Expression body)
        {
            var memberExpression = Expression.Property(body, PropertyInfo);
            if (!PropertyInfo.PropertyType.GetTypeInfo().IsValueType)
            {
                return memberExpression;
            }

            return Expression.Convert(memberExpression, typeof (object));
        }

        public void SetValue(object target, object propertyValue) => PropertyInfo.SetValue(target, propertyValue, null);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(PropertyValueGetter)) return false;
            return Equals((PropertyValueGetter) obj);
        }

        public bool Equals(PropertyValueGetter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.PropertyInfo.PropertyMatches(PropertyInfo);
        }

        public override int GetHashCode() => PropertyInfo?.GetHashCode() ?? 0;
    }
}
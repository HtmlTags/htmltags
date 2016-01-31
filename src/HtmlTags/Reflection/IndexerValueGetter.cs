using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HtmlTags.Reflection
{
    public class IndexerValueGetter : IValueGetter
    {
        public IndexerValueGetter(Type arrayType, int index)
        {
            DeclaringType = arrayType;
            Index = index;
        }

        public object GetValue(object target) => ((Array)target).GetValue(Index);

        public string Name => $"[{Index}]";

        public int Index { get; }

        public Type DeclaringType { get; }

        public Type ValueType => DeclaringType.GetElementType();

        public Expression ChainExpression(Expression body)
        {
            var memberExpression = Expression.ArrayIndex(body, Expression.Constant(Index, typeof(int)));
            if (!DeclaringType.GetElementType().GetTypeInfo().IsValueType)
            {
                return memberExpression;
            }

            return Expression.Convert(memberExpression, typeof(object));
        }

        public void SetValue(object target, object propertyValue) => ((Array)target).SetValue(propertyValue, Index);

        protected bool Equals(IndexerValueGetter other) => DeclaringType == other.DeclaringType && Index == other.Index;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IndexerValueGetter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DeclaringType?.GetHashCode() ?? 0) * 397) ^ Index;
            }
        }
    }



}
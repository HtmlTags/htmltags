using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HtmlTags.Reflection
{
    public class IndexerValueGetter : IValueGetter
    {
        private readonly Type _arrayType;

        public IndexerValueGetter(Type arrayType, int index)
        {
            _arrayType = arrayType;
            Index = index;
        }

        public object GetValue(object target)
        {
            return ((Array)target).GetValue(Index);
        }

        public string Name
        {
            get
            {
                return "[{0}]".ToFormat(Index);
            }
        }

        public int Index { get; private set; }

        public Type DeclaringType
        {
            get { return _arrayType; }
        }

        public Type ValueType
        {
            get { return _arrayType.GetElementType(); }
        }

        public Expression ChainExpression(Expression body)
        {
            var memberExpression = Expression.ArrayIndex(body, Expression.Constant(Index, typeof(int)));
            if (!_arrayType.GetElementType().GetTypeInfo().IsValueType)
            {
                return memberExpression;
            }

            return Expression.Convert(memberExpression, typeof(object));
        }

        public void SetValue(object target, object propertyValue)
        {
            ((Array)target).SetValue(propertyValue, Index);
        }

        protected bool Equals(IndexerValueGetter other)
        {
            return _arrayType == other._arrayType && Index == other.Index;
        }

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
                return ((_arrayType != null ? _arrayType.GetHashCode() : 0) * 397) ^ Index;
            }
        }
    }



}
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HtmlTags.Reflection
{
    public class MethodValueGetter : IValueGetter
    {
        private readonly MethodInfo _methodInfo;
        private readonly object[] _arguments;

        public MethodValueGetter(MethodInfo methodInfo, object[] arguments)
        {
            if (arguments.Length > 1)
            {
                throw new NotSupportedException("ReflectionHelper only supports methods with no arguments or a single indexer argument");
            }

            _methodInfo = methodInfo;
            _arguments = arguments;
        }

        public object GetValue(object target) => _methodInfo.Invoke(target, _arguments);

        public string Name
        {
            get
            {
                if (_arguments.Length == 1) return $"[{_arguments[0]}]";
                if (_arguments.Length == 0) return _methodInfo.Name;

                throw new NotSupportedException("Dunno how to deal with this method");
            }
        }

        public Type DeclaringType
        {
            get { return _methodInfo.DeclaringType; }
        }

        public Type ValueType
        {
            get { return _methodInfo.ReturnType; }
        }

        public Type ReturnType
        {
            get { return _methodInfo.ReturnType; }
        }

        public Expression ChainExpression(Expression body)
        {
            throw new NotSupportedException();
        }

        public void SetValue(object target, object propertyValue)
        {
            throw new NotSupportedException();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(MethodValueGetter)) return false;
            return Equals((MethodValueGetter)obj);
        }

        public bool Equals(MethodValueGetter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._methodInfo, _methodInfo) && Enumerable.SequenceEqual(other._arguments, _arguments);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                if (_arguments.Length != 0)
                {
                    return ((_methodInfo != null ? _methodInfo.GetHashCode() : 0) * 397) ^ (_arguments[0] != null ? _arguments[0].GetHashCode() : 0);
                }

                return _methodInfo.GetHashCode();
            }
        }
    }



}
namespace HtmlTags.Conventions.Elements
{
    using System;
    using System.Linq.Expressions;
    using Reflection;

    public class AccessorDef
    {
        public AccessorDef(Accessor accessor, Type modelType)
        {
            Accessor = accessor;
            ModelType = modelType;
        }

        public Accessor Accessor { get; }
        public Type ModelType { get; }

        public static AccessorDef For<T>(Expression<Func<T, object>> expression)
        {
            return new AccessorDef(expression.ToAccessor(), typeof(T));
        }

        public bool Equals(AccessorDef other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Accessor, Accessor) && Equals(other.ModelType, ModelType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(AccessorDef)) return false;
            return Equals((AccessorDef)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Accessor?.GetHashCode() ?? 0) * 397) ^
                       (ModelType?.GetHashCode() ?? 0);
            }
        }

        public bool Is<T>() => Accessor.PropertyType == typeof(T);
    }
}
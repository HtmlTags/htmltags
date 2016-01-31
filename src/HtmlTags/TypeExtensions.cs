namespace HtmlTags
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class TypeExtensions
    {
        public static T As<T>(this object target) => (T)target;

        public static bool CanBeCastTo<T>(this Type type)
        {
            if (type == null) return false;
            Type destinationType = typeof(T);

            return CanBeCastTo(type, destinationType);
        }

        public static bool CanBeCastTo(this Type type, Type destinationType)
        {
            if (type == null) return false;
            if (type == destinationType) return true;

            return destinationType.IsAssignableFrom(type);
        }

        public static bool IsNullable(this Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool Closes(this Type type, Type openType)
        {
            if (type == null) return false;

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == openType) return true;

            if (type.GetInterfaces().Any(@interface => @interface.Closes(openType)))
            {
                return true;
            }

            Type baseType = type.GetTypeInfo().BaseType;
            if (baseType == null) return false;

            bool closes = baseType.GetTypeInfo().IsGenericType && baseType.GetGenericTypeDefinition() == openType;
            if (closes) return true;

            return type.GetTypeInfo().BaseType != null && type.GetTypeInfo().BaseType.Closes(openType);
        }

        public static Type IsAnEnumerationOf(this Type type)
        {
            if (!type.Closes(typeof(IEnumerable<>)))
            {
                throw new Exception("Duh, its gotta be enumerable");
            }

            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (type.GetTypeInfo().IsGenericType)
            {
                return type.GetGenericArguments()[0];
            }


            throw new Exception(string.Format("I don't know how to figure out what this is a collection of. Can you tell me? {0}", new[] {type}));
        }

        public static bool PropertyMatches(this PropertyInfo prop1, PropertyInfo prop2) 
            => prop1.DeclaringType == prop2.DeclaringType && prop1.Name == prop2.Name;

        public static Type GetInnerTypeFromNullable(this Type nullableType) => nullableType.GetGenericArguments()[0];

        public static bool IsNullableOfT(this Type theType)
        {
            if (theType == null) return false;

            return theType.GetTypeInfo().IsGenericType && theType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        
        public static bool IsTypeOrNullableOf<T>(this Type theType)
        {
            Type otherType = typeof(T);
            return theType == otherType ||
                   (theType.IsNullableOfT() && theType.GetGenericArguments()[0] == otherType);
        }
    }
}
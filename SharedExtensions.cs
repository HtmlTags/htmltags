using System;
using System.Collections.Generic;

namespace HtmlTags
{
    internal static class SharedExtensions
    {
        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }

        public static string ToFormat(this string template, params object[] parameters)
        {
            return string.Format(template, parameters);
        }

        public static string[] ToDelimitedArray(this string content)
        {
            return content.ToDelimitedArray(',');
        }

        public static string[] ToDelimitedArray(this string content, char delimiter)
        {
            string[] array = content.Split(delimiter);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Trim();
            }

            return array;
        }

        public static T As<T>(this object target)
        {
            return (T) target;
        }

        public static bool IsNotEmpty(this string text)
        {
            return !string.IsNullOrEmpty(text);
        }

        public static bool IsEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static T Parse<T>(this string value)
        {
            return (T) Convert.ChangeType(value, typeof (T));
        }

        public static bool IsTrue(this string value)
        {
            return bool.Parse(value);
        }

        public static bool IsTrue(this object value)
        {
            return value is bool ? (bool) value : value.ToString().IsTrue();
        }


        public static string Join(this string[] array, string separator)
        {
            return string.Join(separator, array);
        }

        public static int ToInt(this string stringValue)
        {
            return int.Parse(stringValue);
        }

        public static bool IsSame(this object target, object other)
        {
            return ReferenceEquals(target, other);
        }
    }
}
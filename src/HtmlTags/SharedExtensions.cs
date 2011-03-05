using System;
using System.Collections.Generic;
using System.Linq;

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

        public static string[] ToDelimitedArray(this string content, char delimiter)
        {
            var array = content.Split(delimiter);
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Trim();
            }

            return array;
        }

        public static string Join(this IEnumerable<string> strings, string separator)
        {
#if LEGACY
            return string.Join(separator, strings.ToArray());
#else
            return string.Join(separator, strings);
#endif
        }
    }
}
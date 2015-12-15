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

        public static string[] ToDelimitedArray(this string content, params char[] delimiter) => content.Split(delimiter).Select(x => x.Trim()).ToArray();

        public static string Join(this IEnumerable<string> strings, string separator) => string.Join(separator, strings);
    }
}
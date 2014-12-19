namespace HtmlTags
{
    using System.Collections.Generic;

    internal static class EnumerableExtensions
    {
        public static void Fill<T>(this IList<T> list, T value)
        {
            if (list.Contains(value)) return;
            list.Add(value);
        }
    }
}
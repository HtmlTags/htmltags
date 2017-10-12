using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;

namespace HtmlTags.Testing
{
    public static class ShouldlyExtensions
    {
        public static void ShouldHaveTheSameElementsAs<T>(this IEnumerable<T> items, params T[] elements)
        {
            foreach (var element in elements)
            {
                items.ShouldContain(element);
            }
        }

        public static void ShouldHaveCount<T>(this IEnumerable<T> items, int count)
        {
            items.Count().ShouldBe(count);
        }
    }

    public static class Exception<T> where T : Exception
    {
        public static T ShouldBeThrownBy(Action action) => action.ShouldThrow<T>();
    }
}
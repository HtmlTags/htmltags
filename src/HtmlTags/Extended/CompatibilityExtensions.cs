using System;
using System.Collections.Generic;

namespace HtmlTags.Extended
{
    namespace Compatibility
    {
        public static class CompatibilityExtensions
        {
            [Obsolete("Will be removed by v1.0. use Render(bool) instead.")]
            public static HtmlTag Visible(this HtmlTag tag, bool isVisible)
            {
                tag.Render(isVisible);
                return tag;
            }

            [Obsolete("Will be removed by v1.0. use Render() instead.")]
            public static bool Visible(this HtmlTag tag)
            {
                return tag.Render();
            }

            [Obsolete("Will be removed by 1.0. Use Append(ITagSource) instead.")]
            public static HtmlTag AddChildren(this HtmlTag tag, ITagSource tags)
            {
                return tag.Append(tags);
            }

            [Obsolete("Will be removed by 1.0. Use Append(IEnumerable<HtmlTag>) instead.")]
            public static HtmlTag AddChildren(this HtmlTag tag, IEnumerable<HtmlTag> tags)
            {
                return tag.Append(tags);
            }

            [Obsolete("Will by removed by 1.0. Use Append(HtmlTag) instead.")]
            public static HtmlTag Child(this HtmlTag parent, HtmlTag child)
            {
                return parent.Append(child);
            }

            [Obsolete("Will by removed by 1.0. Use Add<T>() instead.")]
            public static T Child<T>(this HtmlTag tag) where T : HtmlTag, new()
            {
                return tag.Add<T>();
            }
        }
    }
}
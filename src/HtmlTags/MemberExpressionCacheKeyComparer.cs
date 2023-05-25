// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Originally from: https://raw.githubusercontent.com/aspnet/AspNetCore/v3.0.0/src/Mvc/Mvc.ViewFeatures/src/MemberExpressionCacheKeyComparer.cs

using System;
using System.Collections.Generic;

namespace HtmlTags
{
    internal class MemberExpressionCacheKeyComparer : IEqualityComparer<MemberExpressionCacheKey>
    {
        public static readonly MemberExpressionCacheKeyComparer Instance = new();

        public bool Equals(MemberExpressionCacheKey x, MemberExpressionCacheKey y)
        {
            if (x.ModelType != y.ModelType)
            {
                return false;
            }

            var xEnumerator = x.GetEnumerator();
            var yEnumerator = y.GetEnumerator();

            while (xEnumerator.MoveNext())
            {
                if (!yEnumerator.MoveNext())
                {
                    return false;
                }

                // Current is a MemberInfo instance which has a good comparer.
                if (xEnumerator.Current != yEnumerator.Current)
                {
                    return false;
                }
            }

            return !yEnumerator.MoveNext();
        }

        public int GetHashCode(MemberExpressionCacheKey obj)
        {
            var hashCode = new HashCode();
            hashCode.Add(obj.ModelType);

            foreach (var member in obj)
            {
                hashCode.Add(member);
            }

            return hashCode.ToHashCode();
        }
    }
}
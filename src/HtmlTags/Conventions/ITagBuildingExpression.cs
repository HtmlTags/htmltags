using System;

namespace HtmlTags.Conventions
{
    public interface ITagBuildingExpression<T> where T : TagRequest
    {
        CategoryExpression<T> Always { get; }
        CategoryExpression<T> If(Func<T, bool> matches);

        void Add(Func<T, bool> filter, ITagBuilder<T> builder);
        void Add(ITagBuilderPolicy<T> policy);
        void Add(ITagModifier<T> modifier);
    }
}
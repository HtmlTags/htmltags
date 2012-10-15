using System;

namespace HtmlTags.Conventions
{
    public interface ITagBuilderPolicy<T> where T : TagRequest
    {
        bool Matches(T subject);
        ITagBuilder<T> BuilderFor(T subject);
    }

    public class ConditionalTagBuilderPolicy<T> : ITagBuilderPolicy<T> where T : TagRequest
    {
        private readonly Func<T, bool> _filter;
        private readonly ITagBuilder<T> _builder;

        public ConditionalTagBuilderPolicy(Func<T, bool> filter, Func<T, HtmlTag> builder)
        {
            _filter = filter;
            _builder = new LambdaTagBuilder<T>(builder);
        }

        public ConditionalTagBuilderPolicy(Func<T, bool> filter, ITagBuilder<T> builder)
        {
            _filter = filter;
            _builder = builder;
        }


        public bool Matches(T subject)
        {
            return _filter(subject);
        }

        public ITagBuilder<T> BuilderFor(T subject)
        {
            return _builder;
        }
    }

    public interface ITagBuilder<T> where T : TagRequest
    {
        HtmlTag Build(T request);
    }

    public abstract class TagBuilder<T> : ITagBuilderPolicy<T>, ITagBuilder<T> where T : TagRequest
    {
        public abstract bool Matches(T subject);

        public ITagBuilder<T> BuilderFor(T subject)
        {
            return this;
        }

        public abstract HtmlTag Build(T request);
    }
}
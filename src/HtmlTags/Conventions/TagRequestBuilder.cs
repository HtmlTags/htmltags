using System.Collections.Generic;
using System.Linq;

namespace HtmlTags.Conventions
{
    public interface ITagRequestBuilder
    {
        T Build<T>(T tagRequest) where T : TagRequest;
    }

    public class TagRequestBuilder : ITagRequestBuilder
    {
        private readonly IEnumerable<ITagRequestActivator> _activators;

        public TagRequestBuilder(IEnumerable<ITagRequestActivator> activators)
        {
            _activators = activators;
        }

        public T Build<T>(T tagRequest) where T : TagRequest
        {
            _activators
                .Where(x => x.Matches(typeof(T)))
                .Each(a => a.Activate(tagRequest));

            return tagRequest;
        }
    }
}
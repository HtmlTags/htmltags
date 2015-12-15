using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlTags
{
    public class TagList : ITagSource
    {
        private readonly IEnumerable<HtmlTag> _tags;

        public TagList(IEnumerable<HtmlTag> tags)
        {
            _tags = tags;
        }

        public string ToHtmlString()
        {
            if (_tags.Count() > 5)
            {
                var builder = new StringBuilder();
                _tags.Each(t => builder.AppendLine(t.ToString()));

                return builder.ToString();
            }

            return _tags.Select(x => x.ToString()).Join("\n");
        }

        public IEnumerable<HtmlTag> AllTags() => _tags;

        public override string ToString() => ToHtmlString();
    }
}
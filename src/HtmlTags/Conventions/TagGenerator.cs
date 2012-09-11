using System.Collections.Generic;
using System.Linq;

namespace HtmlTags.Conventions
{
    public class TagGenerator<T> : ITagGenerator<T> where T : TagRequest
    {
        private readonly ITagLibrary<T> _library;
        private readonly IEnumerable<ITagRequestActivator> _activators;
        

        public TagGenerator(ITagLibrary<T> library, IEnumerable<ITagRequestActivator> activators)
        {
            ActiveProfile = TagConstants.Default;
            _library = library;
            _activators = activators.Where(x => x.Matches(typeof (T))).ToList();
        }

        public HtmlTag Build(T request, string category = null, string profile = null)
        {
            profile = profile ?? ActiveProfile;
            category = category ?? TagConstants.Default;

            var token = request.ToToken();

            var plan = _library.PlanFor((T)token, profile, category);

            _activators.Each(x => x.Activate(request));

            return plan.Build(request);
        }

        public string ActiveProfile {get; set;}
    }
}
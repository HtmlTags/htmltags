using System.Collections.Generic;
using System.Linq;

namespace HtmlTags.Conventions
{
    public class ActiveProfile
    {
        private readonly Stack<string> _profiles = new Stack<string>();

        public ActiveProfile()
        {
            _profiles.Push(TagConstants.Default);
        }
        
        public string Name
        {
            get { return _profiles.Peek(); }
        }

        public void Push(string profile)
        {
            _profiles.Push(profile);
        }

        public void Pop()
        {
            _profiles.Pop();
        }
    }

    public class TagGenerator<T> : ITagGenerator<T> where T : TagRequest
    {
        private readonly ITagLibrary<T> _library;
        private readonly ActiveProfile _profile;
        private readonly IEnumerable<ITagRequestActivator> _activators;
        

        public TagGenerator(ITagLibrary<T> library, IEnumerable<ITagRequestActivator> activators, ActiveProfile profile)
        {
            _library = library;
            _profile = profile;
            _activators = activators.Where(x => x.Matches(typeof (T))).ToList();
        }

        public HtmlTag Build(T request, string category = null, string profile = null)
        {
            profile = profile ?? _profile.Name ?? TagConstants.Default;
            category = category ?? TagConstants.Default;

            var token = request.ToToken();

            var plan = _library.PlanFor((T)token, profile, category);

            _activators.Each(x => x.Activate(request));

            return plan.Build(request);
        }

        public string ActiveProfile
        {
            get { return _profile.Name; }
        }
    }
}
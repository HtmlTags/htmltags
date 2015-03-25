using System.Collections.Generic;
using System.Linq;

namespace HtmlTags.Conventions
{
    using Elements;

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

    public class TagGenerator : ITagGenerator
    {
        private readonly ITagLibrary _library;
        private readonly ActiveProfile _profile;
        

        public TagGenerator(ITagLibrary library, ActiveProfile profile)
        {
            _library = library;
            _profile = profile;
        }

        public HtmlTag Build(ElementRequest request, string category = null, string profile = null)
        {
            profile = profile ?? _profile.Name ?? TagConstants.Default;
            category = category ?? TagConstants.Default;

            var token = request.ToToken();

            var plan = _library.PlanFor(token, profile, category);

            ActivateRequest(request);

            return plan.Build(request);
        }

        private void ActivateRequest(ElementRequest request)
        {
            //var namingConvention = request.Get<IElementNamingConvention>() ?? new DefaultElementNamingConvention();
            // TODO: add the element id and service location stuff here
            //        request.ElementId = string.IsNullOrEmpty(request.ElementId)
            //? _naming.GetName(request.HolderType(), request.Accessor)
            //: request.ElementId;

        }

        public string ActiveProfile
        {
            get { return _profile.Name; }
        }
    }
}
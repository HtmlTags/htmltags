using System;
using System.Collections.Generic;

namespace HtmlTags.Conventions
{
    public class ActiveProfile
    {
        private readonly Stack<string> _profiles = new Stack<string>();

        public ActiveProfile()
        {
            _profiles.Push(TagConstants.Default);
        }

        public string Name => _profiles.Peek();

        public void Push(string profile) => _profiles.Push(profile);

        public void Pop() => _profiles.Pop();
    }

    public class TagGenerator : ITagGenerator
    {
        private readonly ITagLibrary _library;
        private readonly ActiveProfile _profile;
        private readonly Func<Type, object> _serviceLocator;


        public TagGenerator(ITagLibrary library, ActiveProfile profile, Func<Type, object> serviceLocator)
        {
            _library = library;
            _profile = profile;
            _serviceLocator = serviceLocator;
        }

        public HtmlTag Build(ElementRequest request, string category = null, string profile = null)
        {
            profile = profile ?? _profile.Name ?? TagConstants.Default;
            category = category ?? TagConstants.Default;

            var token = request.ToToken();

            var plan = _library.PlanFor(token, profile, category);

            request.Attach(_serviceLocator);

            return plan.Build(request);
        }

        public string ActiveProfile => _profile.Name;
    }
}
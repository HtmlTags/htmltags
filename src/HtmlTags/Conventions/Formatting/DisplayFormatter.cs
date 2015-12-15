namespace HtmlTags.Conventions.Formatting
{
    using System;
    using Reflection;

    public class DisplayFormatter : IDisplayFormatter
    {
        private readonly Func<Type, object> _locator;
        private readonly Stringifier _stringifier;

        public DisplayFormatter(Func<Type, object> locator)
        {
            _locator = locator;
            _stringifier = new Stringifier();
        }

        public string GetDisplay(GetStringRequest request)
        {
            request.Locator = _locator;
            return _stringifier.GetString(request);
        }

        public string GetDisplay(Accessor accessor, object target)
        {
            var request = new GetStringRequest(accessor, target, _locator, null, null);
            return _stringifier.GetString(request);
        }

        public string GetDisplayForValue(Accessor accessor, object rawValue)
        {
            var request = new GetStringRequest(accessor, rawValue, _locator, null, null);
            return _stringifier.GetString(request);
        }
    }
}
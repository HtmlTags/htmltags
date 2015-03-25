namespace HtmlTags.Conventions.Formatting
{
    using Reflection;

    public interface IDisplayFormatter
    {
        string GetDisplay(GetStringRequest request);
        string GetDisplay(Accessor accessor, object target);
        string GetDisplayForValue(Accessor accessor, object rawValue);
    }
}
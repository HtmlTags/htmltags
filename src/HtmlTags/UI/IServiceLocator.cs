namespace HtmlTags.UI
{
    using System;

    public interface IServiceLocator
    {
        T GetInstance<T>();
        object GetInstance(Type type);
        T GetInstance<T>(string name);
    }
}
//using Bottles;
//using FubuMVC.Core.Registration;
//using HtmlTags.UI.Elements;
//using HtmlTags.UI.Security;
//using HtmlTags.UI.Templates;
//using HtmlTags.Conventions;

//namespace HtmlTags.UI
//{
//    using Elements;
//    using Security;
//    using Templates;

//    public class UIServiceRegistry : ServiceRegistry
//    {
//        public UIServiceRegistry()
//        {
//            SetServiceIfNone<IFieldAccessService, FieldAccessService>();
//            SetServiceIfNone<IFieldAccessRightsExecutor, FieldAccessRightsExecutor>();

//            SetServiceIfNone<IElementNamingConvention, DefaultElementNamingConvention>();

//            AddService<IActivator>(typeof(DisplayConversionRegistryActivator));

//            SetServiceIfNone<IPartialInvoker, PartialInvoker>();

//            SetServiceIfNone(typeof (IElementGenerator<>), typeof (ElementGenerator<>));

//            SetServiceIfNone(typeof (ITagGenerator<>), typeof (TagGenerator<>));

//            AddService<ITagRequestActivator, ElementRequestActivator>();
//            AddService<ITagRequestActivator, ServiceLocatorTagRequestActivator>();

//            SetServiceIfNone<ITagGeneratorFactory, TagGeneratorFactory>();

//            SetServiceIfNone<ITemplateWriter, TemplateWriter>();
            
//            SetServiceIfNone<ITagRequestBuilder, TagRequestBuilder>();
//        }
//    }
//}
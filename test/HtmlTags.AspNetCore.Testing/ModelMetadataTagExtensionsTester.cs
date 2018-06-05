using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HtmlTags.Conventions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders.Testing;
using Moq;
using Shouldly;
using Xunit;

namespace HtmlTags.Testing
{
    public class ModelMetadataTagExtensionsTester
    {
        class Subject
        {
            [Display(Name = "Hello", Prompt = "Value Here")]
            [DisplayFormat(DataFormatString = "Foo {0} Bar", ApplyFormatInEditMode = true, NullDisplayText = "Bunny")]
            [Required]
            [MaxLength(10)]
            public string Value { get; set; }

            public DateTimeOffset DateValue { get; set; }
        }

        [Fact]
        public void ShouldBuildLabelFromDisplayAttribute()
        {
            var subject = new Subject {Value = "Value"};
            var helper = GetHtmlHelper(subject);

            var label = helper.Label(s => s.Value);
            label.Text().ShouldBe("Hello");
        }

        [Fact]
        public void ShouldBuildDisplayFromDisplayFormat()
        {
            var subject = new Subject {Value = "Value"};
            var helper = GetHtmlHelper(subject);

            var display = helper.Display(s => s.Value);
            display.Text().ShouldBe("Foo Value Bar");
        }

        [Fact]
        public void ShouldBuildInputValueFromEditFormat()
        {
            var subject = new Subject {Value = "Value"};
            var helper = GetHtmlHelper(subject);

            var editor = helper.Input(s => s.Value);
            editor.Value().ShouldBe("Foo Value Bar");
        }

        [Fact]
        public void ShouldSetPlaceholderForInput()
        {
            var subject = new Subject {Value = "Value"};
            var helper = GetHtmlHelper(subject);

            var editor = helper.Input(s => s.Value);
            editor.Attr("placeholder").ShouldBe("Value Here");
        }

        [Fact]
        public void ShouldUseNullDisplayTextForDisplay()
        {
            var subject = new Subject {Value = null};
            var helper = GetHtmlHelper(subject);

            var editor = helper.Display(s => s.Value);
            editor.Text().ShouldBe("Foo Bunny Bar");
        }

        [Fact]
        public void ShouldUseNullDisplayTextForEdit()
        {
            var subject = new Subject { Value = null };
            var helper = GetHtmlHelper(subject);

            var editor = helper.Input(s => s.Value);
            editor.Value().ShouldBe("Foo Bunny Bar");
        }

        [Fact]
        public void ShouldAddValidationClassForInvalidValues()
        {
            var subject = new Subject { Value = null };
            var helper = GetHtmlHelper(subject);

            helper.ViewData.ModelState.IsValid.ShouldBeFalse();

            var editor = helper.Input(s => s.Value);
            editor.HasClass(HtmlHelper.ValidationInputCssClassName).ShouldBeTrue();
        }

        [Fact]
        public void ShouldAddClientSideValidationClasses()
        {
            var subject = new Subject { Value = "value" };
            var helper = GetHtmlHelper(subject);

            var editor = helper.Input(s => s.Value);
            editor.Attr("data-val").ShouldBe("true");
            editor.Attr("data-val-maxlength").ShouldNotBeNullOrEmpty();
            editor.Attr("data-val-maxlength-max").ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void ShouldBuildValidationMessage()
        {
            var subject = new Subject { Value = null };
            var helper = GetHtmlHelper(subject);

            var validationMessage = helper.ValidationMessage(s => s.Value);

            validationMessage.TagName().ShouldBe("span");
            validationMessage.Text().ShouldNotBeEmpty();
            validationMessage.HasClass(HtmlHelper.ValidationMessageCssClassName).ShouldBeTrue();
        }

        [Fact]
        public void ShouldHaveEmptyValidationTagWhenNotInvalid()
        {
            var subject = new Subject { Value = "value" };
            var helper = GetHtmlHelper(subject);

            var validationMessage = helper.ValidationMessage(s => s.Value);

            validationMessage.TagName().ShouldBe("span");
            validationMessage.Text().ShouldBeEmpty();
            validationMessage.HasClass(HtmlHelper.ValidationMessageValidCssClassName).ShouldBeTrue();
        }

        [Fact]
        public void ShouldNotAddClientSideValidationClassesWhenNoClientValidationEnabled()
        {
            var subject = new Subject { Value = "value" };
            var helper = GetHtmlHelper(subject);
            helper.ViewContext.ClientValidationEnabled = false;

            var editor = helper.Input(s => s.Value);
            editor.Attr("data-val").ShouldBeNullOrEmpty();
            editor.Attr("data-val-maxlength").ShouldBeNullOrEmpty();
            editor.Attr("data-val-maxlength-max").ShouldBeNullOrEmpty();
        }

        [Fact]
        public void ShouldAllowOverridingOfConventions()
        {
            var subject = new Subject { DateValue = new DateTimeOffset(2018, 1, 1, 12, 00, 00, TimeSpan.FromHours(-6)) };
            var helper = GetHtmlHelper(subject);

            var editor = helper.Input(m => m.DateValue);

            editor.Value().ShouldBe("2018-01-01T12:00");
        }

        public static HtmlHelper<TModel> GetHtmlHelper<TModel>(TModel model)
        {
            return GetHtmlHelper(model, CreateViewEngine());
        }

        public static HtmlHelper<TModel> GetHtmlHelper<TModel>(
            TModel model,
            ICompositeViewEngine viewEngine,
            IStringLocalizerFactory stringLocalizerFactory = null)
        {
            return GetHtmlHelper(
                model,
                CreateUrlHelper(),
                viewEngine,
                TestModelMetadataProvider.CreateDefaultProvider(stringLocalizerFactory));
        }

        public static HtmlHelper<TModel> GetHtmlHelper<TModel>(
            TModel model,
            IUrlHelper urlHelper,
            ICompositeViewEngine viewEngine,
            IModelMetadataProvider provider)
        {
            return GetHtmlHelper(model, urlHelper, viewEngine, provider, innerHelperWrapper: null);
        }

        public static HtmlHelper<TModel> GetHtmlHelper<TModel>(
            TModel model,
            IUrlHelper urlHelper,
            ICompositeViewEngine viewEngine,
            IModelMetadataProvider provider,
            Func<IHtmlHelper, IHtmlHelper> innerHelperWrapper)
        {
            var viewData = new ViewDataDictionary<TModel>(provider, new ModelStateDictionary());
            viewData.Model = model;

            return GetHtmlHelper(
                viewData,
                urlHelper,
                viewEngine,
                provider,
                innerHelperWrapper,
                htmlGenerator: null,
                idAttributeDotReplacement: null);
        }

        private static HtmlHelper<TModel> GetHtmlHelper<TModel>(
    ViewDataDictionary<TModel> viewData,
    IUrlHelper urlHelper,
    ICompositeViewEngine viewEngine,
    IModelMetadataProvider provider,
    Func<IHtmlHelper, IHtmlHelper> innerHelperWrapper,
    IHtmlGenerator htmlGenerator,
    string idAttributeDotReplacement)
        {
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor(), viewData.ModelState);

            var options = new MvcViewOptions();
            if (!string.IsNullOrEmpty(idAttributeDotReplacement))
            {
                options.HtmlHelperOptions.IdAttributeDotReplacement = idAttributeDotReplacement;
            }
            var localizationOptionsAccesor = new Mock<IOptions<MvcDataAnnotationsLocalizationOptions>>();

            localizationOptionsAccesor.SetupGet(o => o.Value).Returns(new MvcDataAnnotationsLocalizationOptions());

            options.ClientModelValidatorProviders.Add(new DataAnnotationsClientModelValidatorProvider(
                new ValidationAttributeAdapterProvider(),
                localizationOptionsAccesor.Object,
                stringLocalizerFactory: null));
            var optionsAccessor = new Mock<IOptions<MvcViewOptions>>();
            optionsAccessor
                .SetupGet(o => o.Value)
                .Returns(options);

            var valiatorProviders = new[]
{
                new DataAnnotationsModelValidatorProvider(
                    new ValidationAttributeAdapterProvider(),
                    new TestOptionsManager<MvcDataAnnotationsLocalizationOptions>(),
                    stringLocalizerFactory: null),
            };

            var validator = new DefaultObjectValidator(provider, valiatorProviders);

            validator.Validate(actionContext, validationState: null, prefix: string.Empty, viewData.Model);

            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            urlHelperFactory
                .Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(urlHelper);

            var expressionTextCache = new ExpressionTextCache();

            var attributeProvider = new DefaultValidationHtmlAttributeProvider(
                optionsAccessor.Object,
                provider,
                new ClientValidatorCache());

            if (htmlGenerator == null)
            {
                htmlGenerator = new DefaultHtmlGenerator(
                    Mock.Of<IAntiforgery>(),
                    optionsAccessor.Object,
                    provider,
                    urlHelperFactory.Object,
                    new HtmlTestEncoder(),
                    attributeProvider);
            }

            // TemplateRenderer will Contextualize this transient service.
            var innerHelper = (IHtmlHelper)new HtmlHelper(
                htmlGenerator,
                viewEngine,
                provider,
                new TestViewBufferScope(),
                new HtmlTestEncoder(),
                UrlEncoder.Default);

            if (innerHelperWrapper != null)
            {
                innerHelper = innerHelperWrapper(innerHelper);
            }

            var serviceCollection = new ServiceCollection();

            serviceCollection
               .AddSingleton(viewEngine)
               .AddSingleton(urlHelperFactory.Object)
               .AddSingleton(Mock.Of<IViewComponentHelper>())
               .AddSingleton(innerHelper)
               .AddSingleton<IViewBufferScope, TestViewBufferScope>()
               .AddSingleton<ValidationHtmlAttributeProvider>(attributeProvider)
               .AddHtmlTags(reg =>
                {
                    reg.Editors.IfPropertyIs<DateTimeOffset>().ModifyWith(m =>
                        m.CurrentTag.Attr("type", "datetime-local")
                            .Value(m.Value<DateTimeOffset?>()?.ToLocalTime().DateTime.ToString("yyyy-MM-ddTHH:mm")));
                });
            
            var serviceProvider = serviceCollection.BuildServiceProvider();

            httpContext.RequestServices = serviceProvider;

            var htmlHelper = new HtmlHelper<TModel>(
                htmlGenerator,
                viewEngine,
                provider,
                new TestViewBufferScope(),
                new HtmlTestEncoder(),
                UrlEncoder.Default,
                expressionTextCache);

            var viewContext = new ViewContext(
                actionContext,
                Mock.Of<IView>(),
                viewData,
                new TempDataDictionary(
                    httpContext,
                    Mock.Of<ITempDataProvider>()),
                new StringWriter(),
                options.HtmlHelperOptions)
            {
                ClientValidationEnabled = true
            };

            htmlHelper.Contextualize(viewContext);

            return htmlHelper;
        }

        private static ICompositeViewEngine CreateViewEngine()
        {
            var view = new Mock<IView>();
            view
                .Setup(v => v.RenderAsync(It.IsAny<ViewContext>()))
                .Callback(async (ViewContext v) =>
                {
                    view.ToString();
                    await v.Writer.WriteAsync(FormatOutput(v.ViewData.ModelExplorer));
                })
                .Returns(Task.FromResult(0));

            var viewEngine = new Mock<ICompositeViewEngine>(MockBehavior.Strict);
            viewEngine
                .Setup(v => v.GetView(/*executingFilePath*/ null, It.IsAny<string>(), /*isMainPage*/ false))
                .Returns(ViewEngineResult.NotFound("MyView", Enumerable.Empty<string>()))
                .Verifiable();
            viewEngine
                .Setup(v => v.FindView(It.IsAny<ActionContext>(), It.IsAny<string>(), /*isMainPage*/ false))
                .Returns(ViewEngineResult.Found("MyView", view.Object))
                .Verifiable();

            return viewEngine.Object;
        }

        private static IUrlHelper CreateUrlHelper()
        {
            return Mock.Of<IUrlHelper>();
        }

        private static string FormatOutput(ModelExplorer modelExplorer)
        {
            var metadata = modelExplorer.Metadata;
            return string.Format(
                CultureInfo.InvariantCulture,
                "Model = {0}, ModelType = {1}, PropertyName = {2}, SimpleDisplayText = {3}",
                modelExplorer.Model ?? "(null)",
                metadata.ModelType == null ? "(null)" : metadata.ModelType.FullName,
                metadata.PropertyName ?? "(null)",
                modelExplorer.GetSimpleDisplayText() ?? "(null)");
        }

        public class TestOptionsManager<TOptions> : IOptions<TOptions>
            where TOptions : class, new()
        {
            public TestOptionsManager()
                : this(new TOptions())
            {
            }

            public TestOptionsManager(TOptions value)
            {
                Value = value;
            }

            public TOptions Value { get; }
        }

        public class TestViewBufferScope : IViewBufferScope
        {
            public IList<ViewBufferValue[]> CreatedBuffers { get; } = new List<ViewBufferValue[]>();

            public IList<ViewBufferValue[]> ReturnedBuffers { get; } = new List<ViewBufferValue[]>();

            public ViewBufferValue[] GetPage(int size)
            {
                var buffer = new ViewBufferValue[size];
                CreatedBuffers.Add(buffer);
                return buffer;
            }

            public void ReturnSegment(ViewBufferValue[] segment)
            {
                ReturnedBuffers.Add(segment);
            }

            public PagedBufferedTextWriter CreateWriter(TextWriter writer)
            {
                return new PagedBufferedTextWriter(ArrayPool<char>.Shared, writer);
            }
        }

        public class TestModelMetadataProvider : DefaultModelMetadataProvider
        {
            // Creates a provider with all the defaults - includes data annotations
            public static IModelMetadataProvider CreateDefaultProvider(IStringLocalizerFactory stringLocalizerFactory = null)
            {
                var detailsProviders = new IMetadataDetailsProvider[]
                {
                new DefaultBindingMetadataProvider(),
                new DefaultValidationMetadataProvider(),
                new DataAnnotationsMetadataProvider(
                    new TestOptionsManager<MvcDataAnnotationsLocalizationOptions>(),
                    stringLocalizerFactory),
                new DataMemberRequiredBindingMetadataProvider(),
                };

                var compositeDetailsProvider = new DefaultCompositeMetadataDetailsProvider(detailsProviders);
                return new DefaultModelMetadataProvider(compositeDetailsProvider, new TestOptionsManager<MvcOptions>());
            }

            public static IModelMetadataProvider CreateDefaultProvider(IList<IMetadataDetailsProvider> providers)
            {
                var detailsProviders = new List<IMetadataDetailsProvider>()
            {
                new DefaultBindingMetadataProvider(),
                new DefaultValidationMetadataProvider(),
                new DataAnnotationsMetadataProvider(
                    new TestOptionsManager<MvcDataAnnotationsLocalizationOptions>(),
                    stringLocalizerFactory: null),
                new DataMemberRequiredBindingMetadataProvider(),
            };

                detailsProviders.AddRange(providers);

                var compositeDetailsProvider = new DefaultCompositeMetadataDetailsProvider(detailsProviders);
                return new DefaultModelMetadataProvider(compositeDetailsProvider, new TestOptionsManager<MvcOptions>());
            }

            public static IModelMetadataProvider CreateProvider(IList<IMetadataDetailsProvider> providers)
            {
                var detailsProviders = new List<IMetadataDetailsProvider>();
                if (providers != null)
                {
                    detailsProviders.AddRange(providers);
                }

                var compositeDetailsProvider = new DefaultCompositeMetadataDetailsProvider(detailsProviders);
                return new DefaultModelMetadataProvider(compositeDetailsProvider, new TestOptionsManager<MvcOptions>());
            }

            private readonly TestModelMetadataDetailsProvider _detailsProvider;

            public TestModelMetadataProvider()
                : this(new TestModelMetadataDetailsProvider())
            {
            }

            private TestModelMetadataProvider(TestModelMetadataDetailsProvider detailsProvider)
                : base(
                      new DefaultCompositeMetadataDetailsProvider(new IMetadataDetailsProvider[]
                      {
                      new DefaultBindingMetadataProvider(),
                      new DefaultValidationMetadataProvider(),
                      new DataAnnotationsMetadataProvider(
                          new TestOptionsManager<MvcDataAnnotationsLocalizationOptions>(),
                          stringLocalizerFactory: null),
                      detailsProvider
                      }),
                      new TestOptionsManager<MvcOptions>())
            {
                _detailsProvider = detailsProvider;
            }

            public IMetadataBuilder ForType(Type type)
            {
                var key = ModelMetadataIdentity.ForType(type);

                var builder = new MetadataBuilder(key);
                _detailsProvider.Builders.Add(builder);
                return builder;
            }

            public IMetadataBuilder ForType<TModel>()
            {
                return ForType(typeof(TModel));
            }

            public IMetadataBuilder ForProperty(Type containerType, string propertyName)
            {
                var property = containerType.GetRuntimeProperty(propertyName);
                Assert.NotNull(property);

                var key = ModelMetadataIdentity.ForProperty(property.PropertyType, propertyName, containerType);

                var builder = new MetadataBuilder(key);
                _detailsProvider.Builders.Add(builder);
                return builder;
            }

            public IMetadataBuilder ForProperty<TContainer>(string propertyName)
            {
                return ForProperty(typeof(TContainer), propertyName);
            }

            private class TestModelMetadataDetailsProvider :
                IBindingMetadataProvider,
                IDisplayMetadataProvider,
                IValidationMetadataProvider
            {
                public List<MetadataBuilder> Builders { get; } = new List<MetadataBuilder>();

                public void CreateBindingMetadata(BindingMetadataProviderContext context)
                {
                    foreach (var builder in Builders)
                    {
                        builder.Apply(context);
                    }
                }

                public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
                {
                    foreach (var builder in Builders)
                    {
                        builder.Apply(context);
                    }
                }

                public void CreateValidationMetadata(ValidationMetadataProviderContext context)
                {
                    foreach (var builder in Builders)
                    {
                        builder.Apply(context);
                    }
                }
            }

            public interface IMetadataBuilder
            {
                IMetadataBuilder BindingDetails(Action<BindingMetadata> action);

                IMetadataBuilder DisplayDetails(Action<DisplayMetadata> action);

                IMetadataBuilder ValidationDetails(Action<ValidationMetadata> action);
            }

            private class MetadataBuilder : IMetadataBuilder
            {
                private List<Action<BindingMetadata>> _bindingActions = new List<Action<BindingMetadata>>();
                private List<Action<DisplayMetadata>> _displayActions = new List<Action<DisplayMetadata>>();
                private List<Action<ValidationMetadata>> _valiationActions = new List<Action<ValidationMetadata>>();

                private readonly ModelMetadataIdentity _key;

                public MetadataBuilder(ModelMetadataIdentity key)
                {
                    _key = key;
                }

                public void Apply(BindingMetadataProviderContext context)
                {
                    if (_key.Equals(context.Key))
                    {
                        foreach (var action in _bindingActions)
                        {
                            action(context.BindingMetadata);
                        }
                    }
                }

                public void Apply(DisplayMetadataProviderContext context)
                {
                    if (_key.Equals(context.Key))
                    {
                        foreach (var action in _displayActions)
                        {
                            action(context.DisplayMetadata);
                        }
                    }
                }

                public void Apply(ValidationMetadataProviderContext context)
                {
                    if (_key.Equals(context.Key))
                    {
                        foreach (var action in _valiationActions)
                        {
                            action(context.ValidationMetadata);
                        }
                    }
                }

                public IMetadataBuilder BindingDetails(Action<BindingMetadata> action)
                {
                    _bindingActions.Add(action);
                    return this;
                }

                public IMetadataBuilder DisplayDetails(Action<DisplayMetadata> action)
                {
                    _displayActions.Add(action);
                    return this;
                }

                public IMetadataBuilder ValidationDetails(Action<ValidationMetadata> action)
                {
                    _valiationActions.Add(action);
                    return this;
                }
            }
        }

    }

}
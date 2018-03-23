namespace HtmlTags
{
    using System;
    using Conventions;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHtmlTags(this IServiceCollection services, HtmlConventionLibrary library) => services.AddSingleton(library);

        public static IServiceCollection AddHtmlTags(this IServiceCollection services, params HtmlConventionRegistry[] registries)
        {
            var library = new HtmlConventionLibrary();
            foreach (var registry in registries)
            {
                registry.Apply(library);
            }
            return services.AddHtmlTags(library);
        }

        public static IServiceCollection AddHtmlTags(this IServiceCollection services, Action<HtmlConventionRegistry> config)
        {
            var registry = new HtmlConventionRegistry();

            config(registry);

            registry.Defaults();
            registry.ModelMetadata();

            return services.AddHtmlTags(registry);
        }
    }
}
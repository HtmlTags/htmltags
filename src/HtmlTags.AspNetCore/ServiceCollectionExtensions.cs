namespace HtmlTags
{
    using System;
    using Conventions;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static void AddHtmlTags(this IServiceCollection services, HtmlConventionLibrary library)
        {
            services.AddSingleton(library);
        }

        public static void AddHtmlTags(this IServiceCollection services, params HtmlConventionRegistry[] registries)
        {
            var library = new HtmlConventionLibrary();
            foreach (var registry in registries)
            {
                registry.Apply(library);
            }
            services.AddHtmlTags(library);
        }

        public static void AddHtmlTags(this IServiceCollection services, Action<HtmlConventionRegistry> config)
        {
            var registry = new HtmlConventionRegistry();

            config(registry);

            services.AddHtmlTags(registry);
        }
    }
}
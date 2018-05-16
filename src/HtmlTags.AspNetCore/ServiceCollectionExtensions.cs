namespace HtmlTags
{
    using System;
    using Conventions;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures HtmlTags without ASP.NET Core defaults without modifying the library
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="library">Convention library</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddHtmlTags(this IServiceCollection services, HtmlConventionLibrary library) => services.AddSingleton(library);

        /// <summary>
        /// Configures HtmlTags with ASP.NET Core defaults
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="registries">Custom convention registries</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddHtmlTags(this IServiceCollection services, params HtmlConventionRegistry[] registries)
        {
            var library = new HtmlConventionLibrary();

            var defaultRegistry = new HtmlConventionRegistry()
                .Defaults()
                .ModelMetadata()
                .ModelState();

            defaultRegistry.Apply(library);

            foreach (var registry in registries)
            {
                registry.Apply(library);
            }

            return services.AddHtmlTags(library);
        }

        /// <summary>
        /// Configures HtmlTags with ASP.NET Core defaults
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="config">Additional configuration callback</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddHtmlTags(this IServiceCollection services, Action<HtmlConventionRegistry> config)
        {
            var registry = new HtmlConventionRegistry();

            config(registry);

            return services.AddHtmlTags(registry);
        }
    }
}
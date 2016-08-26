using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HtmlTags.AspNetCore.TestSite
{
    using System.Reflection;
    using Conventions;
    using Conventions.Elements;
    using Conventions.Elements.Builders;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddHtmlTags(
                reg =>
                {
                    reg.Editors.Always.AddClass("form-control");
                    reg.Editors.BuilderPolicy<EnumDropDownBuilder>();
                    reg.Labels.Always.AddClass("control-label");
                    reg.Labels.Always.AddClass("col-md-2");
                }
            );
        }

        public class EditorConventions : HtmlConventionRegistry
        {
            public EditorConventions()
            {
                Editors.BuilderPolicy<CheckboxBuilder>();

                Editors.Always.BuildBy<TextboxBuilder>();

                Editors.Modifier<AddNameModifier>();

                Displays.Always.BuildBy<SpanDisplayBuilder>();

                Labels.Always.BuildBy<DefaultLabelBuilder>();

                Editors.Always.AddClass("form-control");
                Editors.BuilderPolicy<EnumDropDownBuilder>();

                Labels.Always.AddClass("control-label");
                Labels.Always.AddClass("col-md-2");
            }
        }

        public class EnumDropDownBuilder : ElementTagBuilder
        {
            public override bool Matches(ElementRequest subject)
            {
                return subject.Accessor.PropertyType.GetTypeInfo().IsEnum;
            }

            public override HtmlTag Build(ElementRequest request)
            {
                var enumType = request.Accessor.PropertyType;

                var select = new SelectTag();

                foreach (var value in Enum.GetValues(enumType))
                {
                    select.Option(Enum.GetName(enumType, value), value);
                }
                select.SelectByValue(request.RawValue);

                return select;
            }
        }


        public class DisplayConventions : HtmlConventionRegistry
        {
            
        }

        public class LabelConventions : HtmlConventionRegistry
        {
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}


using System;
using System.Reflection;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;
using HtmlTags.Conventions.Elements.Builders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HtmlTags.AspNetCore.TestSite
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddControllersWithViews();

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}

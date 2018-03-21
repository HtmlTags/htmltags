using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HtmlTags
{
    public static class ModelMetadataTagExtensions
    {
        public static void ModelMetadata(this HtmlConventionRegistry registry)
        {
            registry.Labels.Modifier<DisplayNameElementModifier>();
        }

        private class DisplayNameElementModifier : IElementModifier
        {
            public bool Matches(ElementRequest token) => true;

            public void Modify(ElementRequest request)
            {
                var modelExplorer = request.Get<ModelExplorer>();

                request.CurrentTag.Text(
                    modelExplorer.Metadata.DisplayName ??
                    request.CurrentTag.Text()
                );
            }
        }
    }
}
using System.Globalization;
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
            registry.Displays.Modifier<DisplayFormatStringElementModifier>();
            registry.Editors.Modifier<EditFormatStringElementModifier>();
            registry.Editors.Modifier<PlaceholderElementModifier>();
        }

        private class DisplayNameElementModifier : IElementModifier
        {
            public bool Matches(ElementRequest token) 
                => token.Get<ModelExplorer>()?.Metadata.DisplayName != null;

            public void Modify(ElementRequest request)
                => request.CurrentTag.Text(request.Get<ModelExplorer>().Metadata.DisplayName);
        }

        private class DisplayFormatStringElementModifier : IElementModifier
        {
            public bool Matches(ElementRequest token) 
                => token.Get<ModelExplorer>()?.Metadata.DisplayFormatString != null;

            public void Modify(ElementRequest request) 
                => request.CurrentTag.Text(string.Format(CultureInfo.CurrentCulture, request.Get<ModelExplorer>().Metadata.DisplayFormatString, request.RawValue));
        }

        private class EditFormatStringElementModifier : IElementModifier
        {
            public bool Matches(ElementRequest token) 
                => token.Get<ModelExplorer>()?.Metadata.EditFormatString != null;

            public void Modify(ElementRequest request) 
                => request.CurrentTag.Value(string.Format(CultureInfo.CurrentCulture, request.Get<ModelExplorer>().Metadata.EditFormatString, request.RawValue));
        }

        private class PlaceholderElementModifier : IElementModifier
        {
            public bool Matches(ElementRequest token) 
                => token.Get<ModelExplorer>()?.Metadata.Placeholder != null;

            public void Modify(ElementRequest request) 
                => request.CurrentTag.Attr("placeholder", request.Get<ModelExplorer>().Metadata.Placeholder);
        }
    }
}
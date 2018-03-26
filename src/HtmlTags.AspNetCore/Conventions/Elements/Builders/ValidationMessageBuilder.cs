using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace HtmlTags.Conventions.Elements.Builders
{
    public class DefaultValidationMessageBuilder : IElementBuilder
    {
        public HtmlTag Build(ElementRequest request)
        {
            var viewContext = request.Get<ViewContext>() ?? throw new InvalidOperationException("Validation messages require a ViewContext");

            var formContext = viewContext.ClientValidationEnabled ? viewContext.FormContext : null;
            if (!viewContext.ViewData.ModelState.ContainsKey(request.ElementId) && formContext == null)
            {
                return HtmlTag.Empty();
            }

            var tryGetModelStateResult = viewContext.ViewData.ModelState.TryGetValue(request.ElementId, out var entry);
            var modelErrors = tryGetModelStateResult ? entry.Errors : null;

            ModelError modelError = null;
            if (modelErrors != null && modelErrors.Count != 0)
            {
                modelError = modelErrors.FirstOrDefault(m => !string.IsNullOrEmpty(m.ErrorMessage)) ?? modelErrors[0];
            }

            if (modelError == null && formContext == null)
            {
                return HtmlTag.Empty();
            }

            var tag = new HtmlTag(viewContext.ValidationMessageElement);

            var className = modelError != null ?
                HtmlHelper.ValidationMessageCssClassName :
                HtmlHelper.ValidationMessageValidCssClassName;
            tag.AddClass(className);

            if (modelError != null)
            {
                var modelExplorer = request.Get<ModelExplorer>() ?? throw new InvalidOperationException("Validation messages require a ModelExplorer");
                tag.Text(ValidationHelpers.GetModelErrorMessageOrDefault(modelError, entry, modelExplorer));
            }

            if (formContext != null)
            {
                tag.Attr("data-valmsg-for", request.ElementId);

                tag.Attr("data-valmsg-replace", "true");
            }

            return tag;
        }
    }
}
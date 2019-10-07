using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
                tag.Text(GetModelErrorMessageOrDefault(modelError, entry, modelExplorer));
            }

            if (formContext != null)
            {
                tag.Attr("data-valmsg-for", request.ElementId);

                tag.Attr("data-valmsg-replace", "true");
            }

            return tag;
        }

        // Generously donated by https://github.com/aspnet/AspNetCore/blob/v3.0.0/src/Mvc/Mvc.ViewFeatures/src/ValidationHelpers.cs#L40
        private static string GetModelErrorMessageOrDefault(
            ModelError modelError,
            ModelStateEntry containingEntry,
            ModelExplorer modelExplorer)
        {
            if (!string.IsNullOrEmpty(modelError.ErrorMessage))
            {
                return modelError.ErrorMessage;
            }

            // Default in the ValidationMessage case is a fallback error message.
            var attemptedValue = containingEntry.AttemptedValue ?? "null";
            return modelExplorer.Metadata.ModelBindingMessageProvider.ValueIsInvalidAccessor(attemptedValue);
        }
    }
}
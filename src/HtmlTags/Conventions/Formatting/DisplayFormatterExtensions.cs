namespace HtmlTags.Conventions.Formatting
{
    using System;
    using Reflection;

    public static class DisplayFormatterExtensions
    {
        /// <summary>
        /// Formats the provided value using the accessor accessor metadata and a custom format
        /// </summary>
        /// <param name="formatter">The formatter</param>
        /// <param name="modelType">The type of the model to which the accessor belongs (i.e. Case where the accessor might be on its base class WorkflowItem)</param>
        /// <param name="accessor">The property that holds the given value</param>
        /// <param name="value">The data to format</param>
        /// <param name="format">The custom format specifier</param>
        public static string FormatValue(this IDisplayFormatter formatter, Type modelType, Accessor accessor,
            object value, string format)
        {
            var request = new GetStringRequest(accessor, value, null, format, null);

            return formatter.GetDisplay(request);
        }

        public static string GetDisplayForProperty(this IDisplayFormatter formatter, Accessor accessor, object target)
        {
            return formatter.GetDisplay(accessor, accessor.GetValue(target));
        }

        /// <summary>
        /// Formats the provided value using the property accessor metadata
        /// </summary>
        /// <param name="modelType">The type of the model to which the property belongs (i.e. Case where the property might be on its base class WorkflowItem)</param>
        /// <param name="formatter">The formatter</param>
        /// <param name="property">The property that holds the given value</param>
        /// <param name="value">The data to format</param>
        public static string FormatValue(this IDisplayFormatter formatter, Type modelType, Accessor property,
            object value) => formatter.GetDisplay(new GetStringRequest(property, value, null, null, modelType));

        /// <summary>
        /// Retrieves the formatted value of a property from an instance
        /// </summary>
        /// <param name="formatter">The formatter</param>
        /// <param name="modelType">The type of the model to which the property belongs (i.e. Case where the property might be on its base class WorkflowItem)</param>
        /// <param name="property">The property of <paramref name="entity"/> whose value should be formatted</param>
        /// <param name="entity">The instance containing the data to format</param>
        public static string FormatProperty(this IDisplayFormatter formatter, Type modelType, Accessor property,
            object entity)
        {
            var raw = property.GetValue(entity);
            return formatter.FormatValue(modelType, property, raw);
        }
    }
}
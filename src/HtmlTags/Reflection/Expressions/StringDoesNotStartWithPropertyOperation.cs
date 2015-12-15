using System;
using System.Reflection;

namespace HtmlTags.Reflection.Expressions
{
    public class StringDoesNotStartWithPropertyOperation : CaseInsensitiveStringMethodPropertyOperation
    {
        private static readonly MethodInfo _method =
            ReflectionHelper.GetMethod<string>(s => s.StartsWith("", StringComparison.CurrentCulture));

        public StringDoesNotStartWithPropertyOperation()
            : base(_method, true)
        {
        }

        public override string OperationName => "DoesNotStartWith";

        public override string Text => "does not start with";
    }
}
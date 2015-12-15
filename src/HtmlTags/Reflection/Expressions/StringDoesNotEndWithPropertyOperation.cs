using System;
using System.Reflection;

namespace HtmlTags.Reflection.Expressions
{
    public class StringDoesNotEndWithPropertyOperation : CaseInsensitiveStringMethodPropertyOperation
    {
        private static readonly MethodInfo _method =
            ReflectionHelper.GetMethod<string>(s => s.EndsWith("", StringComparison.CurrentCulture));

        public StringDoesNotEndWithPropertyOperation()
            : base(_method, true)
        {
        }

        public override string OperationName => "DoesNotEndWith";

        public override string Text => "does not end with";
    }
}
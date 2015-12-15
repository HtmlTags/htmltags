using System.Linq.Expressions;

namespace HtmlTags.Reflection.Expressions
{
    public class NotEqualPropertyOperation : BinaryComparisonPropertyOperation
    {
        public NotEqualPropertyOperation()
            : base(ExpressionType.NotEqual)
        {
        }

        public override string OperationName => "IsNot";

        public override string Text => "is not";
    }
}
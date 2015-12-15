using System.Linq.Expressions;

namespace HtmlTags.Reflection.Expressions
{
    public class LessThanOrEqualPropertyOperation : BinaryComparisonPropertyOperation
    {
        public LessThanOrEqualPropertyOperation()
            : base(ExpressionType.LessThanOrEqual)
        {
        }

        public override string OperationName => "LessThanOrEqual";

        public override string Text => "less than or equal to";
    }
}
using System.Linq.Expressions;

namespace HtmlTags.Reflection.Expressions
{
    public class LessThanOrEqualPropertyOperation : BinaryComparisonPropertyOperation
    {
        public LessThanOrEqualPropertyOperation()
            : base(ExpressionType.LessThanOrEqual)
        {
        }

        public override string OperationName { get { return "LessThanOrEqual"; } }

        public override string Text
        {
            get { return "less than or equal to"; }
        }
    }
}
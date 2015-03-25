using System.Linq.Expressions;

namespace HtmlTags.Reflection.Expressions
{
    public class GreaterThanPropertyOperation : BinaryComparisonPropertyOperation
    {
        public GreaterThanPropertyOperation()
            : base(ExpressionType.GreaterThan)
        {
        }

        public override string OperationName { get { return "GreaterThan"; } }
        public override string Text
        {
            get { return "greater than"; }
        }
    }
}
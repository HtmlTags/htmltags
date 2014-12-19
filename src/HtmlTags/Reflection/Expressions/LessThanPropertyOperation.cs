using System.Linq.Expressions;

namespace HtmlTags.Reflection.Expressions
{
    public class LessThanPropertyOperation : BinaryComparisonPropertyOperation
    {
        public LessThanPropertyOperation()
            : base(ExpressionType.LessThan)
        {
        }

        public override string OperationName { get { return "LessThan"; } }

        public override string Text
        {
            get { return "less than"; }
        }
    }
}
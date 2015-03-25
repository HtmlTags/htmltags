using System.Linq.Expressions;

namespace HtmlTags.Reflection.Expressions
{
    public class NotEqualPropertyOperation : BinaryComparisonPropertyOperation
    {
        public NotEqualPropertyOperation()
            : base(ExpressionType.NotEqual)
        {
        }

        public override string OperationName { get { return "IsNot"; } }
        public override string Text
        {
            get { return "is not"; }
        }
    }
}
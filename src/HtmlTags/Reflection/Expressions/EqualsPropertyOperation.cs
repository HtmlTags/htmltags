using System.Linq.Expressions;

namespace HtmlTags.Reflection.Expressions
{
    public class EqualsPropertyOperation : BinaryComparisonPropertyOperation
    {
        public EqualsPropertyOperation()
            : base(ExpressionType.Equal)
        {
        }

        public override string OperationName => "Is";

        public override string Text => "is";
    }
}
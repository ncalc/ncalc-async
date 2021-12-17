using System.Threading.Tasks;

namespace NCalcAsync.Domain
{
    public class UnaryExpression : LogicalExpression
    {
        public UnaryExpression(UnaryExpressionType type, LogicalExpression expression)
        {
            Type = type;
            Expression = expression;
        }

        public LogicalExpression Expression { get; set; }

        public UnaryExpressionType Type { get; set; }

        public override async Task AcceptAsync(LogicalExpressionVisitor visitor)
        {
            await visitor.VisitAsync(this);
        }
    }

    public enum UnaryExpressionType
    {
        Not,
        Negate,
        BitwiseNot,
        Positive
    }
}
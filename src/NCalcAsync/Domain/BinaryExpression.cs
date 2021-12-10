using System.Threading.Tasks;

namespace NCalcAsync.Domain
{
    public class BinaryExpression : LogicalExpression
    {
        public BinaryExpression(BinaryExpressionType type, LogicalExpression leftExpression, LogicalExpression rightExpression)
        {
            Type = type;
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }

        public LogicalExpression LeftExpression { get; set; }

        public LogicalExpression RightExpression { get; set; }

        public BinaryExpressionType Type { get; set; }

        public override async Task AcceptAsync(LogicalExpressionVisitor visitor)
        {
            await visitor.VisitAsync(this);
        }
    }

    public enum BinaryExpressionType
    {
        And,
        Or,
        NotEqual,
        LesserOrEqual,
        GreaterOrEqual,
        Lesser,
        Greater,
        Equal,
        Minus,
        Plus,
        Modulo,
        Div,
        Times,
        BitwiseOr,
        BitwiseAnd,
        BitwiseXOr,
        LeftShift,
        RightShift,
        Unknown,
        Exponentiation
    }
}
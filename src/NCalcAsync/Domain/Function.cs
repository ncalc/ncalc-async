using System.Threading.Tasks;

namespace NCalcAsync.Domain
{
    public class Function : LogicalExpression
    {
        public Function(Identifier identifier, LogicalExpression[] expressions)
        {
            Identifier = identifier;
            Expressions = expressions;
        }

        public Identifier Identifier { get; set; }

        public LogicalExpression[] Expressions { get; set; }

        public override async Task AcceptAsync(LogicalExpressionVisitor visitor)
        {
            await visitor.VisitAsync(this);
        }
    }
}
using System.Threading.Tasks;

namespace NCalcAsync.Domain
{
    public abstract class LogicalExpressionVisitor
    {
        public abstract Task VisitAsync(LogicalExpression expression);
        public abstract Task VisitAsync(TernaryExpression expression);
        public abstract Task VisitAsync(BinaryExpression expression);
        public abstract Task VisitAsync(UnaryExpression expression);
        public abstract Task VisitAsync(ValueExpression expression);
        public abstract Task VisitAsync(Function function);
        public abstract Task VisitAsync(Identifier function);
    }
}
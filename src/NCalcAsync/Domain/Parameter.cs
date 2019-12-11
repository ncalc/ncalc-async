using System.Threading.Tasks;

namespace NCalcAsync.Domain
{
	public class Identifier : LogicalExpression
	{
		public Identifier(string name)
		{
            Name = name;
		}

	    public string Name { get; set; }


	    public override async Task AcceptAsync(LogicalExpressionVisitor visitor)
        {
            await visitor.VisitAsync(this);
        }
    }
}

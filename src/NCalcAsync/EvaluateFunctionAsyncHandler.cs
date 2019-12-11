using System.Threading.Tasks;

namespace NCalcAsync
{
    public delegate Task EvaluateFunctionAsyncHandler(string name, FunctionArgs args);
}

using System.Threading.Tasks;

namespace NCalcAsync
{
    public delegate Task EvaluateParameterAsyncHandler(string name, ParameterArgs args);
}

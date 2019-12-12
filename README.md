[![Build Status](https://dev.azure.com/ncalc-async/ncalc-async/_apis/build/status/ncalc.ncalc-async?branchName=master)](https://dev.azure.com/ncalc-async/ncalc-async/_build/latest?definitionId=1&branchName=master)

# NCalcAsync

NCalcAsync is a fully async port of the [NCalc](https://github.com/ncalc/ncalc) mathematical expressions evaluator in .NET. NCalc can parse any expression and evaluate the result, including static or dynamic parameters and custom functions.

For general documentation refer to the NCalc wiki:
* [description](https://github.com/ncalc/ncalc/wiki/Description): overall concepts, usage and extensibility points
* [operators](https://github.com/ncalc/ncalc/wiki/Operators): available standard operators and structures
* [values](https://github.com/ncalc/ncalc/wiki/Values): authorized values like types, functions, ...
* [functions](https://github.com/ncalc/ncalc/wiki/Functions): list of already implemented functions
* [parameters](https://github.com/ncalc/ncalc/wiki/Parameters): on how to use parameters expressions

## API differences between NCalc and NCalcAsync

Expressions are evaluated using `Expression.EvaluateAsync()` instead of `Expression.Evaluate()`.

Async custom parameter and function handlers are assigned to `Expression.EvaluateParameterAsync` and `Expression.EvalauteFunctionAsync`, instead of adding event handlers.  They are asynchronous and thus return a `Task` but otherwise behave the same way as the handlers in NCalc, i.e. they set `args.Result` to pass back the result and indicate that it handled the symbol.  However, only a single handler each for parameters and functions are supported, where NCalc allows multiple event handlers.  If you need multiple handlers you need to wrap them in an async function calling each in turn.

`FunctionArgs.EvaluateParametersAsync` replaces `FunctionArgs.EvaluateParameters`.

Custom `LogicalExpressionVisitor` implementations must be fully asynchronous.

The custom `LogicalExpression.ToString()` is removed.
# 3.1:

* [New function `Atan2`](https://github.com/ncalc/ncalc-async/pull/15)

# 3.0:

Several syntax changes to the grammar:

* [Exponentiation operator `**`](https://github.com/ncalc/ncalc-async/issues/10)
* [Case insensitive operators and key words (e.g. `AND` or `True`)](https://github.com/ncalc/ncalc/issues/37)
* [Support numbers with trailing dot (e.g. `47.`)](https://github.com/ncalc/ncalc/issues/21)
* [Support for positive sign (e.g. `+5`)](https://github.com/ncalc/ncalc/issues/11)

While these changes in themselves wouldn't introduce compatibility issues with previously valid statements, code that relies on statements with these constructs being invalid would be affected. The grammar also had to be regenerated with a new version of ANTLR with some fixes to it since it was clear that the generated source code had been modified manually. Manual review indicates that the regenerated grammar is identical, but because of both these reasons this is released as a new major version.

# 2.1:

* [Bugfix: invalid tokens were skipped silently without any errors](https://github.com/ncalc/ncalc-async/issues/6). Expressions like `"4711"` would ignore the `"` (since that is not the string character in the NCalc syntax) and parse it as the number `4711`, but now an `EvaluationException` is thrown as for other syntax issues. This may affect existing expressions, but since they were always incorrect and now give an exception rather than silently getting a new value it does not merit a new major release.

# 2.0

* [Major bugfix: long integers are now treated as integers](https://github.com/ncalc/ncalc-async/issues/4). Previous versions converted them to single-precision floats, which caused data loss on large numbers. Since this affects the results of existing expressions, it requires a new major release.

# 1.2

* [New builtin function `Ln()`](https://github.com/ncalc/ncalc-async/pull/2)

# 1.1

* [Handle += for `Expression.EvaluateFunctionAsync` and `Expression.EvaluateParameterAsync`](https://github.com/ncalc/ncalc-async/issues/1)
* Parameter names are changed to lowercase if `EvaluateOptions.IgnoreCase` is set (previously only function names were lowercased)

# 1.0

Initial public release.

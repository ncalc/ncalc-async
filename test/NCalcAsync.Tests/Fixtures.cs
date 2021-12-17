using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NCalcAsync.Tests
{
    [TestClass]
    public class Fixtures
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task ExpressionShouldEvaluate()
        {
            var expressions = new []
            {
                "2 + 3 + 5",
                "2 * 3 + 5",
                "2 * (3 + 5)",
                "2 * (2*(2*(2+1)))",
                "10 % 3",
                "true or false",
                "not true",
                "false || not (false and true)",
                "3 > 2 and 1 <= (3-2)",
                "3 % 2 != 10 % 3"
            };

            foreach (string expression in expressions)
                Console.WriteLine("{0} = {1}",
                    expression,
                    await new Expression(expression).EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldParseValues()
        {
            Assert.AreEqual(123456, await new Expression("123456").EvaluateAsync());
            Assert.AreEqual(new DateTime(2001, 01, 01), await new Expression("#01/01/2001#").EvaluateAsync());
            Assert.AreEqual(0.2d, await new Expression(".2").EvaluateAsync());
            Assert.AreEqual(123.456d, await new Expression("123.456").EvaluateAsync());
            Assert.AreEqual(123d, await new Expression("123.").EvaluateAsync());
            Assert.AreEqual(12300d, await new Expression("123.E2").EvaluateAsync());
            Assert.AreEqual(true, await new Expression("true").EvaluateAsync());
            Assert.AreEqual("true", await new Expression("'true'").EvaluateAsync());
            Assert.AreEqual("azerty", await new Expression("'azerty'").EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldHandleUnicode()
        {
            Assert.AreEqual("経済協力開発機構", await new Expression("'経済協力開発機構'").EvaluateAsync());
            Assert.AreEqual("Hello", await new Expression(@"'\u0048\u0065\u006C\u006C\u006F'").EvaluateAsync());
            Assert.AreEqual("だ", await new Expression(@"'\u3060'").EvaluateAsync());
            Assert.AreEqual("\u0100", await new Expression(@"'\u0100'").EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldEscapeCharacters()
        {
            Assert.AreEqual("'hello'", await new Expression(@"'\'hello\''").EvaluateAsync());
            Assert.AreEqual(" ' hel lo ' ", await new Expression(@"' \' hel lo \' '").EvaluateAsync());
            Assert.AreEqual("hel\nlo", await new Expression(@"'hel\nlo'").EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldDisplayErrorMessages()
        {
            try
            {
                await new Expression("(3 + 2").EvaluateAsync();
                Assert.Fail();
            }
            catch(EvaluationException e)
            {
                Console.WriteLine("Error catched: " + e.Message);
            }
        }

        [TestMethod]
        public async Task Maths()
        {
            Assert.AreEqual(1M, await new Expression("Abs(-1)").EvaluateAsync());
            Assert.AreEqual(0d, await new Expression("Acos(1)").EvaluateAsync());
            Assert.AreEqual(0d, await new Expression("Asin(0)").EvaluateAsync());
            Assert.AreEqual(0d, await new Expression("Atan(0)").EvaluateAsync());
            Assert.AreEqual(2d, await new Expression("Ceiling(1.5)").EvaluateAsync());
            Assert.AreEqual(1d, await new Expression("Cos(0)").EvaluateAsync());
            Assert.AreEqual(1d, await new Expression("Exp(0)").EvaluateAsync());
            Assert.AreEqual(1d, await new Expression("Floor(1.5)").EvaluateAsync());
            Assert.AreEqual(-1d, await new Expression("IEEERemainder(3,2)").EvaluateAsync());
            Assert.AreEqual(0d, await new Expression("Ln(1)").EvaluateAsync());
            Assert.AreEqual(0d, await new Expression("Log(1,10)").EvaluateAsync());
            Assert.AreEqual(0d, await new Expression("Log10(1)").EvaluateAsync());
            Assert.AreEqual(9d, await new Expression("Pow(3,2)").EvaluateAsync());
            Assert.AreEqual(3.22d, await new Expression("Round(3.222,2)").EvaluateAsync());
            Assert.AreEqual(-1, await new Expression("Sign(-10)").EvaluateAsync());
            Assert.AreEqual(0d, await new Expression("Sin(0)").EvaluateAsync());
            Assert.AreEqual(2d, await new Expression("Sqrt(4)").EvaluateAsync());
            Assert.AreEqual(0d, await new Expression("Tan(0)").EvaluateAsync());
            Assert.AreEqual(1d, await new Expression("Truncate(1.7)").EvaluateAsync());
        }

        [TestMethod]
        public async Task ExpressionShouldEvaluateCustomFunctions()
        {
            var e = new Expression("SecretOperation(3, 6)");

            e.EvaluateFunctionAsync = async (name, args) =>
                {
                    if (name == "SecretOperation")
                        args.Result = (int)await args.Parameters[0].EvaluateAsync() + (int)await args.Parameters[1].EvaluateAsync();
                };

            Assert.AreEqual(9, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task ExpressionShouldEvaluateCustomFunctionsWithParameters()
        {
            var e = new Expression("SecretOperation([e], 6) + f");
            e.Parameters["e"] = 3;
            e.Parameters["f"] = 1;

            e.EvaluateFunctionAsync = async (name, args) =>
                {
                    if (name == "SecretOperation")
                        args.Result = (int)await args.Parameters[0].EvaluateAsync() + (int)await args.Parameters[1].EvaluateAsync();
                };

            Assert.AreEqual(10, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task ExpressionShouldEvaluateParameters()
        {
            var e = new Expression("Round(Pow(Pi, 2) + Pow([Pi Squared], 2) + [X], 2)");

            e.Parameters["Pi Squared"] = new Expression("Pi * [Pi]");
            e.Parameters["X"] = 10;

            e.EvaluateParameterAsync = (name, args) =>
                {
                    if (name == "Pi")
                        args.Result = 3.14;

                    return Task.CompletedTask;
                };

            Assert.AreEqual(117.07, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldEvaluateConditionnal()
        {
            var eif = new Expression("if([divider] <> 0, [divided] / [divider], 0)");
            eif.Parameters["divider"] = 5;
            eif.Parameters["divided"] = 5;

            Assert.AreEqual(1d, await eif.EvaluateAsync());

            eif = new Expression("if([divider] <> 0, [divided] / [divider], 0)");
            eif.Parameters["divider"] = 0;
            eif.Parameters["divided"] = 5;
            Assert.AreEqual(0, await eif.EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldOverrideExistingFunctions()
        {
            var e = new Expression("Round(1.99, 2)");

            Assert.AreEqual(1.99d, await e.EvaluateAsync());

            e.EvaluateFunctionAsync = (name, args) =>
            {
                if (name == "Round")
                    args.Result = 3;

                return Task.CompletedTask;
            };

            Assert.AreEqual(3, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldEvaluateInOperator()
        {
            // The last argument should not be evaluated
            var ein = new Expression("in((2 + 2), [1], [2], 1 + 2, 4, 1 / 0)");
            ein.Parameters["1"] = 2;
            ein.Parameters["2"] = 5;

            Assert.AreEqual(true, await ein.EvaluateAsync());

            var eout = new Expression("in((2 + 2), [1], [2], 1 + 2, 3)");
            eout.Parameters["1"] = 2;
            eout.Parameters["2"] = 5;

            Assert.AreEqual(false, await eout.EvaluateAsync());

            // Should work with strings
            var estring = new Expression("in('to' + 'to', 'titi', 'toto')");

            Assert.AreEqual(true, await estring.EvaluateAsync());

        }

        [TestMethod]
        public async Task ShouldEvaluateOperators()
        {
            var expressions = new Dictionary<string, object>
                                  {
                                      {"!true", false},
                                      {"not false", true},
                                      {"Not false", true},
                                      {"NOT false", true},
                                      {"-10", -10},
                                      {"+20", 20},
                                      {"2**-1", 0.5},
                                      {"2**+2", 4.0},
                                      {"2 * 3", 6},
                                      {"6 / 2", 3d},
                                      {"7 % 2", 1},
                                      {"2 + 3", 5},
                                      {"2 - 1", 1},
                                      {"1 < 2", true},
                                      {"1 > 2", false},
                                      {"1 <= 2", true},
                                      {"1 <= 1", true},
                                      {"1 >= 2", false},
                                      {"1 >= 1", true},
                                      {"1 = 1", true},
                                      {"1 == 1", true},
                                      {"1 != 1", false},
                                      {"1 <> 1", false},
                                      {"1 & 1", 1},
                                      {"1 | 1", 1},
                                      {"1 ^ 1", 0},
                                      {"~1", ~1},
                                      {"2 >> 1", 1},
                                      {"2 << 1", 4},
                                      {"true && false", false},
                                      {"True and False", false},
                                      {"tRue aNd faLse", false},
                                      {"TRUE ANd fALSE", false},
                                      {"true AND FALSE", false},
                                      {"true || false", true},
                                      {"true or false", true},
                                      {"true Or false", true},
                                      {"true OR false", true},
                                      {"if(true, 0, 1)", 0},
                                      {"if(false, 0, 1)", 1}
                                  };

            foreach (KeyValuePair<string, object> pair in expressions)
            {
                Assert.AreEqual(pair.Value, await new Expression(pair.Key).EvaluateAsync(), pair.Key + " failed");
            }

        }

        [TestMethod]
        public async Task ShouldHandleOperatorsPriority()
        {
            Assert.AreEqual(8, await new Expression("2+2+2+2").EvaluateAsync());
            Assert.AreEqual(16, await new Expression("2*2*2*2").EvaluateAsync());
            Assert.AreEqual(6, await new Expression("2*2+2").EvaluateAsync());
            Assert.AreEqual(6, await new Expression("2+2*2").EvaluateAsync());

            Assert.AreEqual(9d, await new Expression("1 + 2 + 3 * 4 / 2").EvaluateAsync());
            Assert.AreEqual(13.5, await new Expression("18/2/2*3").EvaluateAsync());

            Assert.AreEqual(-1d, await new Expression("-1 ** 2").EvaluateAsync());
            Assert.AreEqual(1d, await new Expression("(-1) ** 2").EvaluateAsync());
            Assert.AreEqual(512d, await new Expression("2 ** 3 ** 2").EvaluateAsync());
            Assert.AreEqual(64d, await new Expression("(2 ** 3) ** 2").EvaluateAsync());
            Assert.AreEqual(18d, await new Expression("2 * 3 ** 2").EvaluateAsync());
            Assert.AreEqual(8d, await new Expression("2 ** 4 / 2").EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldNotLoosePrecision()
        {
            Assert.AreEqual(0.5, await new Expression("3/6").EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldThrowAnExpcetionWhenInvalidNumber()
        {
            try
            {
                await new Expression(". + 2").EvaluateAsync();
                Assert.Fail();
            }
            catch (EvaluationException e)
            {
                Console.WriteLine("Error catched: " + e.Message);
            }
        }

        [TestMethod]
        public async Task ShouldNotRoundDecimalValues()
        {
            Assert.AreEqual(false, await new Expression("0 <= -0.6").EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldEvaluateTernaryExpression()
        {
            Assert.AreEqual(1, await new Expression("1+2<3 ? 3+4 : 1").EvaluateAsync());
        }


        [TestMethod]
        public async Task ShouldHandleStringConcatenation()
        {
            Assert.AreEqual("toto", await new Expression("'to' + 'to'").EvaluateAsync());
            Assert.AreEqual("one2", await new Expression("'one' + 2").EvaluateAsync());
            Assert.AreEqual(3M, await new Expression("1 + '2'").EvaluateAsync());
        }

        [TestMethod]
        public void ShouldDetectSyntaxErrorsBeforeEvaluation()
        {
            var e = new Expression("a + b * (");
            Assert.IsNull(e.Error);
            Assert.IsTrue(e.HasErrors());
            Assert.IsTrue(e.HasErrors());
            Assert.IsNotNull(e.Error);

            e = new Expression("* b ");
            Assert.IsNull(e.Error);
            Assert.IsTrue(e.HasErrors());
            Assert.IsNotNull(e.Error);
        }

        [TestMethod]
        public void ShouldReuseCompiledExpressionsInMultiThreadedMode()
        {
            Parallel.For(0, 10000, async i =>
            {
                var expression = new Expression((i % 111).ToString());

                var result = await expression.EvaluateAsync();
                Assert.AreEqual(i % 111, result);
            });
        }

        [TestMethod]
        public async Task ShouldHandleCaseSensitiveness()
        {
            Assert.AreEqual(1M, await new Expression("aBs(-1)", EvaluateOptions.IgnoreCase).EvaluateAsync());
            Assert.AreEqual(1M, await new Expression("Abs(-1)", EvaluateOptions.None).EvaluateAsync());

            try
            {
                Assert.AreEqual(1M, await new Expression("aBs(-1)", EvaluateOptions.None).EvaluateAsync());
            }
            catch (ArgumentException)
            {
                return;
            }
            catch (Exception)
            {
                Assert.Fail("Unexpected exception");
            }

            Assert.Fail("Should throw ArgumentException");
        }

        [TestMethod]
        public async Task ShouldHandleCustomParametersWhenNoSpecificParameterIsDefined()
        {
            var e = new Expression("Round(Pow([Pi], 2) + Pow([Pi], 2) + 10, 2)");

            e.EvaluateParameterAsync = (name, arg) =>
            {
                if (name == "Pi")
                    arg.Result = 3.14;

                return Task.CompletedTask;
            };

            await e.EvaluateAsync();
        }

        [TestMethod]
        public async Task ShouldHandleCustomFunctionsInFunctions()
        {
            var e = new Expression("if(true, func1(x) + func2(func3(y)), 0)");

            e.EvaluateFunctionAsync = async (name, arg) =>
            {
                switch (name)
                {
                    case "func1": arg.Result = 1;
                        break;
                    case "func2": arg.Result = 2 * Convert.ToDouble(await arg.Parameters[0].EvaluateAsync());
                        break;
                    case "func3": arg.Result = 3 * Convert.ToDouble(await arg.Parameters[0].EvaluateAsync());
                        break;
                }
            };

            e.EvaluateParameterAsync = (name, arg) =>
            {
                switch (name)
                {
                    case "x": arg.Result = 1;
                        break;
                    case "y": arg.Result = 2;
                        break;
                    case "z": arg.Result = 3;
                        break;
                }

                return Task.CompletedTask;
            };

            Assert.AreEqual(13d, await e.EvaluateAsync());
        }


        [TestMethod]
        public async Task ShouldParseScientificNotation()
        {
            Assert.AreEqual(12.2d, await new Expression("1.22e1").EvaluateAsync());
            Assert.AreEqual(100d, await new Expression("1e2").EvaluateAsync());
            Assert.AreEqual(100d, await new Expression("1e+2").EvaluateAsync());
            Assert.AreEqual(0.01d, await new Expression("1e-2").EvaluateAsync());
            Assert.AreEqual(0.001d, await new Expression(".1e-2").EvaluateAsync());
            Assert.AreEqual(10000000000d, await new Expression("1e10").EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldEvaluateArrayParameters()
        {
            var e = new Expression("x * x", EvaluateOptions.IterateParameters);
            e.Parameters["x"] = new [] { 0, 1, 2, 3, 4 };

            var result = (IList)await e.EvaluateAsync();

            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(1, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(9, result[3]);
            Assert.AreEqual(16, result[4]);
        }

        [TestMethod]
        public async Task CustomFunctionShouldReturnNull()
        {
            var e = new Expression("SecretOperation(3, 6)");

            e.EvaluateFunctionAsync = (name, args) =>
            {
                Assert.IsFalse(args.HasResult);
                if (name == "SecretOperation")
                    args.Result = null;
                Assert.IsTrue(args.HasResult);

                return Task.CompletedTask;
            };

            Assert.AreEqual(null, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task CustomParametersShouldReturnNull()
        {
            var e = new Expression("x");

            e.EvaluateParameterAsync = (name, args) =>
            {
                Assert.IsFalse(args.HasResult);
                if (name == "x")
                    args.Result = null;
                Assert.IsTrue(args.HasResult);

                return Task.CompletedTask;
            };

            Assert.AreEqual(null, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldCompareDates()
        {
            Assert.AreEqual(true, await new Expression("#1/1/2009#==#1/1/2009#").EvaluateAsync());
            Assert.AreEqual(false, await new Expression("#2/1/2009#==#1/1/2009#").EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldRoundAwayFromZero()
        {
            Assert.AreEqual(22d, await new Expression("Round(22.5, 0)").EvaluateAsync());
            Assert.AreEqual(23d, await new Expression("Round(22.5, 0)", EvaluateOptions.RoundAwayFromZero).EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldEvaluateSubExpressions()
        {
            var volume = new Expression("[surface] * h");
            var surface = new Expression("[l] * [L]");
            volume.Parameters["surface"] = surface;
            volume.Parameters["h"] = 3;
            surface.Parameters["l"] = 1;
            surface.Parameters["L"] = 2;

            Assert.AreEqual(6, await volume.EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldHandleLongValues()
        {
            Assert.AreEqual(40_000_000_000 + 1, await new Expression("40000000000+1").EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldCompareLongValues()
        {
            Assert.AreEqual(false, await new Expression("(0=1500000)||(((0+2200000000)-1500000)<0)").EvaluateAsync());
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public async Task ShouldDisplayErrorIfUncompatibleTypes()
        {
            var e = new Expression("(a > b) + 10");
            e.Parameters["a"] = 1;
            e.Parameters["b"] = 2;
            await e.EvaluateAsync();
        }

        [TestMethod]
        public async Task ShouldNotConvertRealTypes()
        {
            var e = new Expression("x/2");
            e.Parameters["x"] = 2F;
            Assert.AreEqual(typeof(float), (await e.EvaluateAsync()).GetType());

            e = new Expression("x/2");
            e.Parameters["x"] = 2D;
            Assert.AreEqual(typeof(double), (await e.EvaluateAsync()).GetType());

            e = new Expression("x/2");
            e.Parameters["x"] = 2m;
            Assert.AreEqual(typeof(decimal), (await e.EvaluateAsync()).GetType());

            e = new Expression("a / b * 100");
            e.Parameters["a"] = 20M;
            e.Parameters["b"] = 20M;
            Assert.AreEqual(100M, await e.EvaluateAsync());

        }

        [TestMethod]
        public async Task ShouldShortCircuitBooleanExpressions()
        {
            var e = new Expression("([a] != 0) && ([b]/[a]>2)");
            e.Parameters["a"] = 0;

            Assert.AreEqual(false, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldAddDoubleAndDecimal()
        {
            var e = new Expression("1.8 + Abs([var1])");
            e.Parameters["var1"] = 9.2;

            Assert.AreEqual(11M, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldSubtractDoubleAndDecimal()
        {
            var e = new Expression("1.8 - Abs([var1])");
            e.Parameters["var1"] = 0.8;

            Assert.AreEqual(1M, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldMultiplyDoubleAndDecimal()
        {
            var e = new Expression("1.8 * Abs([var1])");
            e.Parameters["var1"] = 9.2;

            Assert.AreEqual(16.56M, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldDivideDoubleAndDecimal()
        {
            var e = new Expression("1.8 / Abs([var1])");
            e.Parameters["var1"] = 0.5;

            Assert.AreEqual(3.6M, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task ShouldHandleDelayedAsyncFunctionsAndParameters()
        {
            var e = new Expression("delay(200) + delay(fixed_delay)")
            {
                EvaluateFunctionAsync = async (name, args) =>
                {
                    if (name == "delay")
                    {
                        var ms = (int) await args.Parameters[0].EvaluateAsync();
                        await Task.Delay(ms);

                        args.Result = ms;
                        args.HasResult = true;
                    }
                },

                EvaluateParameterAsync = async (name, args) =>
                {
                    if (name == "fixed_delay")
                    {
                        var ms = 100;
                        await Task.Delay(ms);

                        args.Result = ms;
                        args.HasResult = true;
                    }
                }
            };

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = await e.EvaluateAsync();
            stopWatch.Stop();

            Assert.AreEqual(300, result);

            // It's difficult to do exact timing tests in async code, but since delay(fixed_delay) will delay at least 200ms,
            // and delay(200) will do the same, we know it should take at least 200ms to run.
            Assert.IsTrue(stopWatch.ElapsedMilliseconds >= 200);
        }

        [TestMethod]
        public async Task MultipleEvaluateFunctionHandlersCanBeAdded()
        {
            var e = new Expression("a(10) + b(20)");

            // Delaying in the first function handler that's added but not in the second
            // seems to reliably cause a() not to be found when the code doesn't handle
            // MulticastDelegate itself.  (Timing-dependent unit tests are always suspect, though).
            e.EvaluateFunctionAsync += async (name, args) =>
            {
                await Task.Delay(200);
                if (name == "a")
                {
                    args.Result = 10 * (int) await args.Parameters[0].EvaluateAsync();
                }
            };

            e.EvaluateFunctionAsync += async (name, args) =>
            {
                if (name == "b")
                {
                    args.Result = 100 * (int) await args.Parameters[0].EvaluateAsync();
                }
            };

            Assert.AreEqual(2100, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task MultipleEvaluateParameterHandlersCanBeAdded()
        {
            var e = new Expression("a + b");

            // Same kind of timing as in the test above
            e.EvaluateParameterAsync += async (name, args) =>
            {
                await Task.Delay(200);
                if (name == "a")
                {
                    args.Result = 10;
                }
            };

            e.EvaluateParameterAsync += (name, args) =>
            {
                if (name == "b")
                {
                    args.Result = 20;
                }

                return Task.CompletedTask;
            };

            Assert.AreEqual(30, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task IncorrectCalculation_Issue_4()
        {
            Expression e = new Expression("(1604326026000-1604325747000)/60000");
            var evalutedResult = await e.EvaluateAsync();

            Assert.IsInstanceOfType(evalutedResult, typeof(double));
            Assert.AreEqual(4.65, (double)evalutedResult, 0.001);
        }

        [TestMethod]
        public void Should_Throw_Exception_On_Lexer_Errors_Issue_6()
        {
            // https://github.com/ncalc/ncalc-async/issues/6

            var result1 = Assert.ThrowsException<EvaluationException>(() => Expression.Compile("\"0\"", true));
            Assert.AreEqual("line 1:1 no viable alternative at character '\"'", result1.Message);

            var result2 = Assert.ThrowsException<EvaluationException>(() => Expression.Compile("Format(\"{0:(###) ###-####}\", \"9999999999\")", true));
            Assert.AreEqual("line 1:8 no viable alternative at character '\"'", result2.Message);
        }

        [TestMethod]
        public async Task Should_Divide_Decimal_By_Double_Issue_16()
        {
            // https://github.com/ncalc/ncalc/issues/16

            var e = new Expression("x / 1.0");
            e.Parameters["x"] = 1m;

            Assert.AreEqual(1m, await e.EvaluateAsync());
        }

        [TestMethod]
        public async Task Should_Divide_Decimal_By_Single()
        {
            var e = new Expression("x / y");
            e.Parameters["x"] = 1m;
            e.Parameters["y"] = 1f;

            Assert.AreEqual(1m, await e.EvaluateAsync());
        }
    }
}


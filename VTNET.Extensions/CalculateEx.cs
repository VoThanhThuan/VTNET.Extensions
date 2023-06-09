using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VTNET.Extensions.Model;

namespace VTNET.Extensions
{
    public static class CalculateEx
    {
        static HashSet<string> _functions = new HashSet<string> { "log", "log10", "sin", "cos", "tan", "sqrt", "abs", "%", "!" };
        static Dictionary<string, int> _operatorPriority = new Dictionary<string, int> { { "^", 3 }, { "*", 2 }, { "/", 2 }, { "+", 1 }, { "-", 1 } };
        static Dictionary<string, int> _operatorPriorityPlus = new Dictionary<string, int>();
        static string _patternSI = @"(\d+)\s*(rad|deg)";
        static string _patternNumber = @"(?:[^()]+|\((?:[^()]+|\([^()]\))\))";
        //static string _patternFunction = $"(?<function>{string.Join("|", _functions)})";
        static string _patternFunction = $@"(?<function>\w+)";
        static string _patternArg = $"(?<arg>{_patternNumber}+)";
        static string _variables = $"(?<var>pi|e)";
        static string _pattern = $"^{_patternFunction}\\({_patternArg}\\)|{_variables}";
        static string _patterns = $"{_patternFunction}\\({_patternArg}\\)|{_variables}";
        static Dictionary<string, CalculateFuntionCustom> _FunctionExtensions = new Dictionary<string, CalculateFuntionCustom>();
        static Dictionary<string, Func<double, double, double>> _FunctionOperators = new Dictionary<string, Func<double, double, double>>();
        public static void AddSimpleFunction(string functionName, Func<double, double> function)
        {
            _FunctionExtensions.Add(functionName, new CalculateFuntionCustom(x => {
                return function(x[0]);
            }));
        }
        public static void AddFunction(string functionName, Func<double[], double> function, int parameterRequired = 0)
        {
            _FunctionExtensions.Add(functionName, new CalculateFuntionCustom(x =>
            {
                return function(x);
            }, parameterRequired));
        }
        public static void RemoveFunction(string functionName)
        {
            _FunctionExtensions.Remove(functionName);
        }
        public static void AddOperator(char functionName, Func<double, double, double> function, int priority = 1)
        {
            var name = functionName.ToString();
            _FunctionOperators.Add(name, function);
            if (!_operatorPriority.ContainsKey(name))
            {
                _operatorPriorityPlus.Add(name, priority);
                _operatorPriority.Add(name, priority);
            }
        }
        public static void RemoveOperator(string functionName)
        {
            _FunctionOperators.Remove(functionName);
            if (_operatorPriorityPlus.ContainsKey(functionName))
            {
                _operatorPriorityPlus.Remove(functionName);
                _operatorPriority.Remove(functionName);
            }
        }
        public static string GetCustomFunction()
        {
            return string.Join(',', _FunctionExtensions);
        }        
        public static string GetCustomOperator()
        {
            return string.Join(',', _operatorPriorityPlus);
        }
        static double CalculateSimple(ref string strMath, ref Stack<double> values, ref Stack<string> operators)
        {
            var stringValue = "";
            var isGroup = false;
            var result = 0.0d;
            var loop = true;
            var loopCurrent = 0;
            var checkFunction = Regex.Match(strMath, _patterns);
            var loopMax = checkFunction.Index;
            // Operator
            while (!string.IsNullOrEmpty(strMath) && loop)
            {
                var op = strMath[..1];
                strMath = strMath[1..];
                switch (op)
                {
                    case "(":
                        isGroup = true;
                        break;
                    case ")":
                        isGroup = false;
                        if (!string.IsNullOrEmpty(stringValue))
                        {
                            if (double.TryParse(stringValue, out var num))
                            {
                                values.Push(num);
                            }
                        }
                        while (values.Count > 1 && operators.Count > 0)
                        {
                            values.Push(ExecuteOperation(operators.Pop(), values.Pop(), values.Pop()));
                        }
                        break;
                    case "!":
                        {
                            if (double.TryParse(stringValue, out var value))
                            {
                                var fact = 1;
                                for (var i = 2; i <= value; i++)
                                {
                                    fact *= i;
                                }
                                value = fact;
                                values.Push(value);
                                stringValue = "";
                            }
                            else
                            {
                                return double.NaN;
                            }
                        }
                        break;
                    case "%":
                        {
                            if (double.TryParse(stringValue, out var value))
                            {
                                value /= 100;
                                values.Push(value);
                                stringValue = "";
                            }
                            else
                            {
                                return double.NaN;
                            }
                        }
                        break;
                    default:
                        {
                            if (_operatorPriority.TryGetValue(op, out var value))
                            {
                                if (!string.IsNullOrEmpty(stringValue))
                                {
                                    if (double.TryParse(stringValue, out var num))
                                    {
                                        values.Push(num);
                                    }
                                    else
                                    {
                                        return double.NaN;
                                    }
                                    stringValue = "";
                                }

                                while (!isGroup && values.Count > 1 && operators.Count > 0 && value <= _operatorPriority[operators.Peek()])
                                {
                                    result = ExecuteOperation(operators.Pop(), values.Pop(), values.Pop());
                                    values.Push(result);
                                }

                                operators.Push(op);
                            }
                            else
                            {
                                stringValue += op;
                            }

                        }
                        break;
                }

                if(loopMax > 0 && ++loopCurrent >= loopMax)
                {
                    loop = false;
                }
            }
            if (!string.IsNullOrEmpty(stringValue))
            {
                if (double.TryParse(stringValue, out var num))
                {
                    values.Push(num);
                }
            }
            while (operators.Count > 0 && values.Count > 1)
            {
                result = ExecuteOperation(operators.Pop(), values.Pop(), values.Pop());
                values.Push(result);
            }
            return result;
        }

        /// <summary>
        /// String Math Calculator
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static double Calculate(this string input)
        {
            var operators = new Stack<string>();
            var values = new Stack<double>();

            var stringValue = "";
            //var isGroup = false;

            while (!string.IsNullOrEmpty(input))
            {
                var match = Regex.Match(input, _pattern);
                if (match.Success)
                {
                    var function = match.Groups["function"].Value;
                    var arg = match.Groups["arg"].Value;
                    var variable = match.Groups["var"].Value;
                    var isDeg = false;
                    if (arg.EndsWith("deg"))
                    {
                        arg = arg[..^3];
                        isDeg = true;
                    }

                    var matchSI = Regex.Match(arg, _patternSI);
                    var functionValid = true;
                    double value = 0;
                    if (matchSI.Success)
                    {
                        functionValid = double.TryParse(matchSI.Groups[1].Value, out value);
                    }                    
                    else if (!_FunctionExtensions.ContainsKey(function))
                    {
                        var matchChild = Regex.Match(arg, _pattern);
                        if (matchChild.Success)
                        {
                            value = arg.Calculate();
                            functionValid = !double.IsNaN(value);
                        }
                        else if (variable == "pi")
                        {
                            value = Math.PI;
                        }
                        else if (variable == "e")
                        {
                            value = Math.PI;
                        }
                        else
                        {
                            functionValid = double.TryParse(arg, out value);
                            if (!functionValid)
                            {
                                value = CalculateSimple(ref arg, ref values, ref operators);
                                if (!double.IsNaN(value))
                                {
                                    functionValid = true;
                                    value = values.Pop();
                                }
                            }
                        }

                    }
                    if (functionValid)
                    {
                        switch (function)
                        {
                            case "log":
                                value = Math.Log10(value);
                                break;
                            case "log10":
                                value = Math.Log10(value);
                                break;
                            case "sin":
                                value = isDeg ? Math.Sin(value * Math.PI / 180) : Math.Sin(value);
                                break;
                            case "cos":
                                value = isDeg ? Math.Cos(value * Math.PI / 180) : Math.Cos(value);
                                break;
                            case "tan":
                                value = isDeg ? Math.Tan(value * Math.PI / 180) : Math.Tan(value);
                                break;
                            case "sqrt":
                                value = Math.Sqrt(value);
                                break;
                            case "abs":
                                value = Math.Abs(value);
                                break;
                            case "%":
                                value /= 100;
                                break;
                            case "!":
                                var fact = 1;
                                for (var i = 2; i <= value; i++)
                                {
                                    fact *= i;
                                }
                                value = fact;
                                break;
                            case "pi":
                                value = Math.PI;
                                break;
                            case "e":
                                value = Math.E;
                                break;
                            default:
                                var valueArg = arg.Split(',')
                                    .Select(item => double.TryParse(item, out var num) ? num : double.NaN)
                                    .ToArray();
                                value = _FunctionExtensions[function].Function(valueArg);
                                break;
                        }
                        values.Push(value);
                        input = input[match.Length..];
                    }
                    else
                    {
                        // Invalid argument
                        return double.NaN;
                    }
                }
                else
                {                    
                    CalculateSimple(ref input, ref values, ref operators);
                }
            }
            if (!string.IsNullOrEmpty(stringValue))
            {
                if (double.TryParse(stringValue, out var num))
                {
                    values.Push(num);
                }
            }
            while (operators.Count > 0 && values.Count > 1)
            {
                values.Push(ExecuteOperation(operators.Pop(), values.Pop(), values.Pop()));
            }

            if (values.Count != 1)
            {
                // Invalid expression
                return double.NaN;
            }

            return values.Pop();

        }
        private static double ExecuteOperation(string op, double b, double a)
        {
            return op switch
            {
                "^" => Math.Pow(a, b),
                "*" => a * b,
                "/" => a / b,
                "+" => a + b,
                "-" => a - b,
                _ => _FunctionOperators.Any() && _FunctionOperators.ContainsKey(op) ? _FunctionOperators[op](a, b) : double.NaN
            };
        }
    }
}

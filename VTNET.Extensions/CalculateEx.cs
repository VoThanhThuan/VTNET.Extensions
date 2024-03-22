using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VTNET.Extensions.Models;

namespace VTNET.Extensions
{
    public static class CalculateEx
    {
        static Dictionary<string, int> _operatorPriority = new() { { "^", 3 }, { "*", 2 }, { "/", 2 }, { "+", 1 }, { "-", 1 } };
        static Dictionary<string, int> _operatorPriorityPlus = new();
        static string _patternSI = @"(\d+)\s*(rad|deg)";
        static string _patternNumber = @"(?:[^()]+|\((?:[^()]+|\([^()]\))\))";
        static string _patternFunction = $@"(?<function>\w+)";
        static string _patternArg = $"(?<arg>{_patternNumber}+)";
        static string _variables = $"(?<var>pi|e)";
        static string _pattern = $"^{_patternFunction}\\({_patternArg}\\)|{_variables}";
        static string _patterns = $"{_patternFunction}\\({_patternArg}\\)|{_variables}";
        static Dictionary<string, CalculateFuntionCustom> _FunctionExtensions = new();
        static Dictionary<string, Func<double, double, double>> _FunctionOperators = new();
        static Dictionary<string, double> _VariablesDefine = new() { { "pi", Math.PI}, { "e", Math.E} };
        static List<string> _VariablesOrder = new() { "pi", "e" };
        static CultureInfo _calculateCulture = CultureInfo.InvariantCulture;
        public static CultureInfo Culture { get => _calculateCulture; set => _calculateCulture = value; }

        static string VaribaleBuilder()
        {
            var list = new List<string>() { "(?<var>" };
            foreach (var item in _VariablesDefine)
            {
                list.Add(item.Key);
            }
            list.Add(")");
            return "|".Join(list);
        }
        public static void AddVariable(string key, double value)
        {
            _VariablesDefine.Add(key, value);
            _VariablesOrder.Add(key);
            _VariablesOrder.Sort((a, b) => b.CompareTo(a));
            _variables = VaribaleBuilder();
        }
        static string ReplaceVariable(string input, CultureInfo calculateCulture)
        {
            foreach (var key in _VariablesOrder)
            {
                input = input.Replace(key, _VariablesDefine[key].ToString(calculateCulture));
            }
            return input;
        }
        /// <summary>
        /// Add your own custom function.
        /// <code>
        /// CalculateEx.AddSimpleFunction("addone", num =>
        /// {
        ///    return ++num;
        /// });
        /// </code>
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="function"></param>
        public static void AddSimpleFunction(string functionName, Func<double, double> function)
        {
            _FunctionExtensions.Add(functionName, new CalculateFuntionCustom((x, isDeg) => {
                return function(x[0]);
            }));
        }

        /// <summary>
        /// <para>(support Degree and Radian)</para>
        /// Add your own custom function.
        /// <code>
        /// CalculateEx.AddSimpleFunction("circle", (num, isDeg) =>
        /// {
        ///     return isDeg? num*360 : ++num;
        /// });
        /// </code>
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="function"></param>
        public static void AddSimpleFunction(string functionName, Func<double, bool, double> function)
        {
            _FunctionExtensions.Add(functionName, new CalculateFuntionCustom((x, isDeg) => {
                return function(x[0], isDeg);
            }));
        }


        /// <summary>
        /// Add your own custom function. Allows multiple parameters to be passed, note: parameters are separated by ";" ex: sum(1;2;3;4;5;6)
        /// <code>
        /// CalculateEx.AddFunction("sum", agrs =>
        /// {
        ///    return agrs.Sum();
        /// });
        /// </code>
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="function"></param>
        /// <param name="parameterRequired">number of required parameters to be passed</param>
        public static void AddFunction(string functionName, Func<double[], double> function, int parameterRequired = 0)
        {
            _FunctionExtensions.Add(functionName, new CalculateFuntionCustom((args, isDeg)=>function(args), parameterRequired));
        }
        /// <summary>
        /// <para>(support Degree and Radian)</para>
        /// Add your own custom function. Allows multiple parameters to be passed, note: parameters are separated by ";" ex: sum(1;2;3;4;5;6deg)
        /// <code>
        ///CalculateEx.AddFunction("circleSum", (agrs, isDeg) =>
        /// {
        ///     return isDeg? agrs.Sum() * 360 : agrs.Sum();
        /// });
        /// </code>
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="function"></param>
        /// <param name="parameterRequired">number of required parameters to be passed</param>
        public static void AddFunction(string functionName, Func<double[], bool, double> function, int parameterRequired = 0)
        {
            _FunctionExtensions.Add(functionName, new CalculateFuntionCustom(function, parameterRequired));
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
        static double CalculateSimple(ref string strMath, Stack<double> values, Stack<string> operators, CultureInfo? calculateCulture = null)
        {
            calculateCulture ??= _calculateCulture;
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
                            if (double.TryParse(stringValue, style: NumberStyles.Any, provider: calculateCulture, out var num))
                            {
                                values.Push(num);
                                stringValue = "";
                            }
                        }
                        while (values.Count > 1 && operators.Count > 0)
                        {
                            values.Push(ExecuteOperation(operators.Pop(), values.Pop(), values.Pop()));
                        }
                        break;
                    case "!":
                        {
                            if (double.TryParse(stringValue, style: NumberStyles.Any, provider: calculateCulture, out var value))
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
                            if (double.TryParse(stringValue, style: NumberStyles.Any, provider: calculateCulture, out var value))
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
                                    if (double.TryParse(stringValue, style: NumberStyles.Any, provider: calculateCulture, out var num))
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
                if (double.TryParse(stringValue, style: NumberStyles.Any, provider: calculateCulture, out var num))
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
        public static double Calculate(this string input, CultureInfo? calculateCulture = null)
        {
            calculateCulture ??= _calculateCulture;
            input = ReplaceVariable(input, calculateCulture);
            input = input.RemoveSpaces();
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

                    var matchSI = Regex.Match(arg, _patternSI);
                    var functionValid = true;
                    double value = 0;
                    if (matchSI.Success)
                    {
                        if (arg.EndsWith("deg"))
                        {
                            arg = arg[..^3];
                            isDeg = true;
                        }
                        functionValid = double.TryParse(matchSI.Groups[1].Value, style: NumberStyles.Any, provider: calculateCulture, out value);
                    }                    
                    else if (!_FunctionExtensions.ContainsKey(function))
                    {
                        var matchChild = Regex.Match(arg, _pattern);
                        if (matchChild.Success)
                        {
                            value = arg.Calculate();
                            functionValid = !double.IsNaN(value);
                        }
                        //else if (variable == "pi")
                        //{
                        //    value = Math.PI;
                        //}
                        //else if (variable == "e")
                        //{
                        //    value = Math.E;
                        //}
                        else
                        {
                            functionValid = double.TryParse(arg, style: NumberStyles.Any, provider: calculateCulture, out value);
                            if (!functionValid)
                            {
                                value = CalculateSimple(ref arg, values, operators, calculateCulture);
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
                            //case "pi":
                            //    value = Math.PI;
                            //    break;
                            //case "e":
                            //    value = Math.E;
                            //    break;
                            default:
                                Stack<double> valuesClone = new Stack<double>(values);
                                var valueArg = arg.Split(';')
                                    .Select(item => double.TryParse(item, style: NumberStyles.Any, provider: calculateCulture, out var num) ? num : CalculateSimple(ref item, valuesClone, operators))
                                    .ToArray();
                                CalculateFuntionCustom func = _FunctionExtensions[function];
                                if (valueArg.Length < func.ParameterRequired)
                                {
                                    return double.NaN;
                                }
                                else
                                {
                                    value = func.Function(valueArg, isDeg);
                                }
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
                    CalculateSimple(ref input, values, operators, calculateCulture);
                }
            }
            if (!string.IsNullOrEmpty(stringValue))
            {
                if (double.TryParse(stringValue, style: NumberStyles.Any, provider: calculateCulture, out var num))
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

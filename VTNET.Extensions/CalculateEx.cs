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
        static Dictionary<string, Func<decimal?, decimal?, decimal?>> _FunctionOperators = new();
        static Dictionary<string, decimal> _VariablesDefine = new() { { "pi", (decimal)Math.PI}, { "e", (decimal)Math.E} };
        static List<string> _VariablesOrder = new() { "pi", "e" };
        static HashSet<string> _listChar = new() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "(", ")", "%", "!" };
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
        public static void AddVariable(string key, decimal value)
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
        public static void AddSimpleFunction(string functionName, Func<decimal?, decimal?> function)
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
        public static void AddSimpleFunction(string functionName, Func<decimal?, bool, decimal?> function)
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
        public static void AddFunction(string functionName, Func<decimal?[], decimal?> function, int parameterRequired = 0)
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
        public static void AddFunction(string functionName, Func<decimal?[], bool, decimal?> function, int parameterRequired = 0)
        {
            _FunctionExtensions.Add(functionName, new CalculateFuntionCustom(function, parameterRequired));
        }

        public static void RemoveFunction(string functionName)
        {
            _FunctionExtensions.Remove(functionName);
        }
        public static void AddOperator(char functionName, Func<decimal?, decimal?, decimal?> function, int priority = 1)
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
        static readonly Dictionary<string, string> _mapOperator = new()
        {
            {"--", "+" },
            {"-+", "-" },
            {"++", "+" }
        };
        static bool JoinOperators(string op1, string op2, out string opJoin)
        {
            var pairOperator = $"{op1}{op2}";
            if (_mapOperator.TryGetValue(pairOperator, out var c))
            {
                opJoin = c;
                return true;
            }
            opJoin = pairOperator;
            return false;
        }
        static string ReplaceOperators(string input)
        {
            foreach (var kvp in _mapOperator)
            {
                input = input.Replace(kvp.Key, kvp.Value);
            }
            return input;
        }
        /// <summary>
        /// Handles the case where a new calculation starts and the operator comes first
        /// </summary>
        /// <param name="strMath"></param>
        /// <returns></returns>
        static string FirstHanlder(ref string strMath)
        {
            var stringValue = "";
            // Peek Operator handler
            var peekValue = strMath[0].ToString();
            if (_operatorPriority.TryGetValue(peekValue, out _))
            {
                stringValue = strMath[0].ToString();
                strMath = strMath[1..];
            }
            return stringValue;
        }
        static bool CheckDoupleOperators(string strMath)
        {
            if (strMath.Length < 2) return false;
            var first = strMath[0].ToString();
            var second = strMath[1].ToString();
            var count = 0;
            foreach (var item in _operatorPriority)
            {
                if(item.Key == first || item.Key == second)
                {
                    count++;
                }
            }
            return count > 1;
        }
        static decimal? CalculateSimple(ref string strMath, Stack<decimal?> values, Stack<string> operators, CultureInfo? calculateCulture = null)
        {
            calculateCulture ??= _calculateCulture;
            var stringValue = FirstHanlder(ref strMath);
            var isGroup = false;
            decimal? result = null;
            var loop = true;
            var loopCurrent = 0;
            var checkFunction = Regex.Match(strMath, _patterns);
            var loopMax = checkFunction.Index;
            // Operator handler
            while (!string.IsNullOrEmpty(strMath) && loop)
            {
                var op = strMath[..1];               
                strMath = strMath[1..];
                switch (op)
                {
                    case "(":
                        isGroup = true;
                        // Peek Operator handler
                        stringValue = FirstHanlder(ref strMath);
                        break;
                    case ")":
                        isGroup = false;
                        if (!string.IsNullOrEmpty(stringValue))
                        {
                            if (decimal.TryParse(stringValue, style: NumberStyles.Any, provider: calculateCulture, out var num))
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
                            if (decimal.TryParse(stringValue, style: NumberStyles.Any, provider: calculateCulture, out var value))
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
                                return null;
                            }
                        }
                        break;
                    case "%":
                        {
                            if (decimal.TryParse(stringValue, style: NumberStyles.Any, provider: calculateCulture, out var value))
                            {
                                value /= 100;
                                values.Push(value);
                                stringValue = "";
                            }
                            else
                            {
                                return null;
                            }
                        }
                        break;
                    default:
                        {
                            if (_operatorPriority.TryGetValue(op, out var value))
                            {
                                if (!string.IsNullOrEmpty(stringValue))
                                {
                                    if (decimal.TryParse(stringValue, style: NumberStyles.Any, provider: calculateCulture, out var num))
                                    {
                                        values.Push(num);
                                    }
                                    else
                                    {
                                        return null;
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
                if (decimal.TryParse(stringValue, style: NumberStyles.Any, provider: calculateCulture, out var num))
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
        public static decimal? CalculateM(this string input, CultureInfo? calculateCulture = null)
        {
            calculateCulture ??= _calculateCulture;
            input = ReplaceOperators(input);
            input = ReplaceVariable(input, calculateCulture);
            input = input.RemoveSpaces();
            var operators = new Stack<string>();
            var values = new Stack<decimal?>();

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
                    //var functionValid = true;
                    decimal? value = 0m;
                    if (matchSI.Success)
                    {
                        if (arg.EndsWith("deg"))
                        {
                            arg = arg[..^3];
                            isDeg = true;
                        }
                        _ = NumberEx.ParseDecimal(matchSI.Groups[1].Value, style: NumberStyles.Any, provider: calculateCulture, out value);
                    }                    
                    else if (!_FunctionExtensions.ContainsKey(function))
                    {
                        var matchChild = Regex.Match(arg, _pattern);
                        if (matchChild.Success)
                        {
                            value = arg.CalculateM();
                            //functionValid = value is not null;
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
                            _ = NumberEx.ParseDecimal(arg, style: NumberStyles.Any, provider: calculateCulture, out value);
                            if (value is null)
                            {
                                value = CalculateSimple(ref arg, values, operators, calculateCulture);
                                if (value is not null)
                                {
                                    //functionValid = true;
                                    value = values.Pop();
                                }
                            }
                        }

                    }
                    if (value is not null)
                    {
                        switch (function)
                        {
                            case "log":
                                value = NumberEx.Log10(value);
                                break;
                            case "log10":
                                value = NumberEx.Log10(value);
                                break;
                            case "sin":
                                value = isDeg ? NumberEx.Sin(value * NumberEx.PI / 180) : NumberEx.Sin(value);
                                break;
                            case "cos":
                                value = isDeg ? NumberEx.Cos(value * NumberEx.PI / 180) : NumberEx.Cos(value);
                                break;
                            case "tan":
                                value = isDeg ? NumberEx.Tan(value * NumberEx.PI / 180) : NumberEx.Tan(value);
                                break;
                            case "sqrt":
                                value = NumberEx.Sqrt(value);
                                break;
                            case "abs":
                                value = NumberEx.Abs(value);
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
                                Stack<decimal?> valuesClone = new(values);
                                var valueArg = arg.Split(';')
                                    .Select(item => decimal.TryParse(item, style: NumberStyles.Any, provider: calculateCulture, out var num) ? num : CalculateSimple(ref item, valuesClone, operators))
                                    .ToArray();
                                CalculateFuntionCustom func = _FunctionExtensions[function];
                                if (valueArg.Length < func.ParameterRequired)
                                {
                                    return null;
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
                        return null;
                    }
                }
                else
                {
                    var op = input[0].ToString();
                    if (_operatorPriority.TryGetValue(op, out var _) && values.Count > 0)
                    {
                        input = input[1..];
                        operators.Push(op);
                    }
                    else
                    {
                        CalculateSimple(ref input, values, operators, calculateCulture);
                    }
                }
            }
            if (!string.IsNullOrEmpty(stringValue))
            {
                if (decimal.TryParse(stringValue, style: NumberStyles.Any, provider: calculateCulture, out var num))
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
                return null;
            }

            return values.Pop();

        }
        private static decimal? ExecuteOperation(string op, decimal? b, decimal? a)
        {
            return op switch
            {
                "^" => NumberEx.Pow(a, b),
                "*" => a * b,
                "/" => a / b,
                "+" => a + b,
                "-" => a - b,
                _ => _FunctionOperators.Any() && _FunctionOperators.ContainsKey(op) ? _FunctionOperators[op](a, b) : null
            };
        }

        /// <summary>
        /// String Math Calculator
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static double Calculate(this string input, CultureInfo? calculateCulture = null)
        {
            var result = CalculateM(input, calculateCulture);
            if (result == null)
                return double.NaN;
            return (double)result;
        }
    }
}

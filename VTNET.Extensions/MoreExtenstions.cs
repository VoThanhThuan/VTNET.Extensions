using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using VTNET.Extensions.Languages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;

namespace VTNET.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class MoreExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static bool IsNumber(this object variable)
        {
            return variable is sbyte || variable is byte ||
               variable is short || variable is ushort ||
               variable is int || variable is uint ||
               variable is long || variable is ulong ||
               variable is float || variable is double || variable is decimal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumericString(this string str)
        {
            return double.TryParse(str, out var _);
        }

        /// <summary>
        /// True if value is greater than zero and non-empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsTrue(this object? value)
        {
            if (value is null)
            {
                return false;
            }

            if(value is bool boolean)
            {
                return boolean;
            }
            else if (value is string str)
            {
                if((double.TryParse(str, out var num) && num < 1))
                {
                    return false;
                }
                else {
                    return !string.IsNullOrWhiteSpace(str);
                }
            }
            else if (value.IsNumber())
            {
                var number = Convert.ToDouble(value);
                if (number < 1)
                {
                    return false;
                }
            }
            else if (value is Array array)
            {
                return array.Length > 1;
            }
            else if (value is ICollection collection && collection.Count < 1)
            {
                return false;
            }
            else if (value is Enum && value.GetHashCode() == 0)
            {
                return false;
            }
            else if (value is Guid guid && guid == Guid.Empty)
            {
                return false;
            }
            else if (value is DateTime dt && dt == DateTime.MinValue)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// False if value is less than zero and empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsFalse(this object? value)
        {
            return !IsTrue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool IsEven(this object? number)
        {
            if (number?.IsNumber() == true)
            {
                var numericValue = Convert.ToInt64(number);
                return numericValue % 2 == 0;
            }

            throw new InvalidOperationException($"Number of type '{number?.GetType()}' is not supported.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsOdd(this object? number)
        {
            return !number.IsEven();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static dynamic? ToDynamic(this object value)
        {
            IDictionary<string, object?> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                expando.Add(property.Name, property.GetValue(value));

            return expando as ExpandoObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="matchCase"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable table, bool matchCase = false) where T : new()
        {
            var list = new List<T>();
            var properties = typeof(T).GetProperties()
                .ToDictionary(GetColumnName, prop => prop.Name, matchCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

            foreach (DataRow row in table.Rows)
            {
                var item = new T();

                foreach (DataColumn column in table.Columns)
                {
                    if (properties.TryGetValue(column.ColumnName, out var propertyName) && row[column] != DBNull.Value)
                    {
                        var propertyInfo = typeof(T).GetProperty(propertyName);
                        propertyInfo?.SetValue(item, Convert.ChangeType(row[column], propertyInfo.PropertyType));
                    }
                }

                list.Add(item);
            }
            return list;
        }

        private static string GetColumnName(PropertyInfo prop)
        {
            var columnNameAttribute = prop.GetCustomAttribute<ColumnNameAttribute>();
            return columnNameAttribute?.Name ?? prop.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> ToListParallel<T>(this DataTable table) where T : new()
        {
            var list = new List<T>();
            var properties = typeof(T).GetProperties()
                .ToDictionary(prop => prop.GetCustomAttribute<ColumnNameAttribute>()?.Name ?? prop.Name, prop => prop.Name);

            _ = Parallel.ForEach(table.Rows.Cast<DataRow>(), row =>
            {
                var item = new T();
                foreach (DataColumn column in table.Columns)
                {
                    if (properties.TryGetValue(column.ColumnName, out var value) && row[column] != DBNull.Value)
                    {
                        var propertyInfo = typeof(T).GetProperty(value);
                        propertyInfo?.SetValue(item, Convert.ChangeType(row[column], propertyInfo.PropertyType), null);
                    }
                }
                lock (list)
                {
                    list.Add(item);
                }
            });

            return list;
        }
        /// <summary>
        /// <para>Will rely on "separation condition" to group from "first element that meets the condition" to "the next element that meets the condition"</para>
        /// <para>For example: {1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0}</para>
        /// <para>Separation conditions: SplitGroup(x => x == 1)</para>
        /// <para>After separating:</para>
        /// <para>List 1: {1, 0, 0, 0, 0}</para>
        /// <para>List 2: {1, 0, 0, 0}</para>
        /// <para>List 3: {1, 0, 0}</para>
        /// <para>List 4: {1, 0}</para>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<List<T>> SplitGroup<T, U>(this IEnumerable<T> list, Func<T, U> selector)
        {
            List<List<T>> result = new List<List<T>>();
            List<T> sublist = new List<T>();
            U defaultValue = default;
            foreach (T item in list)
            {
                U fieldValue = selector(item);
                if (!fieldValue!.Equals(defaultValue) && sublist.Count > 0)
                {
                    result.Add(sublist);
                    sublist = new List<T>();
                }
                sublist.Add(item);
            }
            if (sublist.Count > 0)
            {
                result.Add(sublist);
            }
            return result;
        }

        /// <summary>
        /// Split a list into multiple lists
        /// <para>For example: {1, 2, 2, 2, 2, 1, 3, 3, 3, 1, 4, 4, 1, 5}</para>
        /// <para>Separation conditions: SplitGroup(x => x == 1)</para>
        /// <para>After separating:</para>
        /// <para>List 1: {2, 2, 2, 2}</para>
        /// <para>List 2: {3, 3, 3}</para>
        /// <para>List 3: {4, 4}</para>
        /// <para>List 4: {5}</para>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<List<T>> Split<T>(this List<T> source, Func<T, string> selector)
        {
            var result = new List<List<T>>();
            var tempDict = new Dictionary<string, List<T>>();

            foreach (var item in source)
            {
                var key = selector(item);
                if (!tempDict.ContainsKey(key))
                {
                    tempDict[key] = new List<T>();
                }

                tempDict[key].Add(item);
            }

            result.AddRange(tempDict.Values);

            return result;
        }

        /// <summary>
        /// Chuyển chữ tiếng việt có dấu thành không dấu
        /// </summary>
        /// <returns>Tiếng việt không dấu và được xóa bỏ những khoảng trắng thừa</returns>
        public static string ConvertToUnSign(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            var temp = s.Normalize(NormalizationForm.FormD);
            var textUnSign = regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            return RemoveDuplicateSpaces(textUnSign);
        }


        static readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static string ToCurrency(this double s, char separator = ',', int? digits = null)
        {
            if (double.IsNaN(s) || double.IsInfinity(s))
            {
                return s.ToString();
            }

            var format = digits == null ? "N" : $"N{digits}";

            var numberFormat = culture.NumberFormat;
            var thousandSeparator = numberFormat.NumberGroupSeparator[0];
            var decimalSeparator = numberFormat.NumberDecimalSeparator[0];

            if (separator == thousandSeparator)
            {
                return s.ToString(format, culture);
            }

            var result = new StringBuilder(s.ToString(format, culture));
            var secondChar = separator == decimalSeparator ? thousandSeparator : decimalSeparator;

            result.Replace(thousandSeparator.ToString(), "🐼").Replace(decimalSeparator, secondChar);
            result.Replace("🐼", separator.ToString());

            return result.ToString();
        }

        /// <summary>
        /// Thousand separator for numbers
        /// </summary>
        public static string ToCurrency(this float s, char separator = ',', int? digits = null)
        {
            return ToCurrency((double)s, separator, digits);
        }

        /// <summary>
        /// Thousand separator for numbers
        /// </summary>
        public static string ToCurrency(this int s, char separator = ',', int? digits = null)
        {
            return ToCurrency((float)s, separator, digits);
        }

        /// <summary>
        /// Thousand separator for numbers
        /// </summary>
        public static string ToCurrency(this long s, char separator = ',', int? digits = null)
        {
            return ToCurrency((double)s, separator, digits);
        }

        /// <summary>
        /// Thousand separator for numbers
        /// </summary>
        public static string ToCurrency(this string s, char separator = ',', int? digits = null)
        {
            return ToCurrency(double.Parse(s), separator, digits);
        }

        /// <summary>
        /// Thousand separator for numbers
        /// </summary>
        public static string ToWords(this int number)
        {
            return ToWords((long)number);
        }

        /// <summary>
        /// Thousand separator for numbers
        /// </summary>
        public static string ToWords(this long number)
        {
            if (number == 0)
            {
                return $"{LanguageNumberWords.Language["Zero"]} ";
            }

            if (number < 0)
            {
                return $"{LanguageNumberWords.Language["Minus"]} " + ToWords(Math.Abs(number));
            }

            string words = "";

            if ((number / 1_000_000_000) > 0)
            {
                words += ToWords(number / 1_000_000_000) + $"{LanguageNumberWords.Language["Billion"]} ";
                number %= 1_000_000_000;
            }

            if ((number / 1_000_000) > 0)
            {
                words += ToWords(number / 1_000_000) + $"{LanguageNumberWords.Language["Million"]} ";
                number %= 1_000_000;
            }

            if ((number / 1000) > 0)
            {
                words += ToWords(number / 1000) + $"{LanguageNumberWords.Language["Thousand"]} ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ToWords(number / 100) + $"{LanguageNumberWords.Language["Hundred"]} ";
                number %= 100;
            }

            if (number > 0)
            {
                string[] unitsMap = LanguageNumberWords.UnitsMap;
                string[] tensMap = LanguageNumberWords.TensMap;

                if (number < 20)
                {
                    words += $"{unitsMap[number]} ";
                }
                else
                {
                    words += $"{tensMap[number / 10]} ";
                    if ((number % 10) > 0)
                    {
                        words += $"{unitsMap[number % 10]} ";
                    }
                }
            }

            return words;
        }

        /// <summary>
        /// Remove extra white space
        /// <para>For example: "Here    is    an     example"</para>
        /// </summary>
        /// <param name="text">"Here is an example"</param>
        /// <returns></returns>
        public static string RemoveDuplicateSpaces(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            while (text.Contains("  "))
                text = text.Replace("  ", " ");
            return text;
        }

        /// <summary>
        /// Capitalize first letter
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Capitalize(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (text.Length == 1) return text.ToUpper();
            return char.ToUpper(text[0]) + text.Substring(1);
        }

        /// <summary>
        /// Capitalize the first letter of each word
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Title(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (text.Length == 1) return text.ToUpper();
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(text);
        }

        /// <summary>
        /// Similar <see cref="string.Join(string?, IEnumerable{string?})"/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Join(this string text, IEnumerable<string> values)
        {
            return string.Join(text, values);
        }

        /// <summary>
        /// Similar <see cref="string.Format(string, object?[]) "/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string text, params object?[] args)
        {
            return string.Format(text, args);
        }

        /// <summary>
        /// Similar <see cref="string.IsNullOrEmpty(string?)"/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string? text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string? text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        /// <summary>
        /// Đây là Console.WriteLine()
        /// </summary>
        /// <param name="text"></param>
        public static void Log(this object obj)
        {
            Console.WriteLine(obj);
        }
        
        public static void Log(this object obj, params object[] objArray)
        {
            var str = new StringBuilder();
            str.Append(obj);
            foreach (var item in objArray)
            {
                str.Append(item);
            }
            Console.WriteLine(str);
        }

        /// <summary>
        /// Make lorem
        /// </summary>
        /// <param name="text"></param>
        /// <param name="minWords">Minimum number of words</param>
        /// <param name="maxWords">Maximum number of words</param>
        /// <param name="minSentences">Minimum number of sentences</param>
        /// <param name="maxSentences">Maximum number of sentences</param>
        /// <param name="numParagraphs"></param>
        /// <returns></returns>
        public static string Lorem(this string text, int minWords = 4, int maxWords = 20, int minSentences = 0, int maxSentences = 1, int numParagraphs = 1)
        {
            if (!string.IsNullOrEmpty(text)) return text;

            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
            "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
            "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat",
            "amet", "do", "tempor", "incididunt", "labore", "et", "magna", "aliqua", "vothuan"};

            if (minSentences > maxSentences)
            {
                maxSentences = minSentences;
            }
            if (minWords > maxWords)
            {
                maxWords = minWords;
            }
            if (numParagraphs < 0)
            {
                numParagraphs = 1;
            }
            var rand = new Random(Guid.NewGuid().GetHashCode());
            var numSentences = rand.Next(maxSentences - minSentences)
                + minSentences + 1;

            var result = new List<string>() {
                "Lorem ",
                "ipsum "
            };
            for (int p = 0; p < numParagraphs; p++)
            {
                var wordAfterDot = "";
                for (int s = 0; s < numSentences; s++)
                {
                    var numWords = rand.Next(maxWords - minWords) + minWords + 1;
                    for (int w = 0; w < numWords - 1; w++)
                    {
                        if (w > 0) result.Add(" ");
                        var word = words[rand.Next(words.Length)];
                        while (result.Last().Equals(word))
                        {
                            word = words[rand.Next(words.Length)];
                        }
                        result.Add(word);
                    }
                    if (numSentences - 1 > s)
                    {
                        result.Add($", ");
                    }
                }
                if (numParagraphs - 1 > p)
                {
                    wordAfterDot = " " + words[rand.Next(words.Length)].Capitalize();
                }
                result.Add($".{wordAfterDot}");
            }
            return "".Join(result);
        }

        static HashSet<string> _functions = new HashSet<string> { "log", "log10", "sin", "cos", "tan", "sqrt", "abs", "%", "!" };
        static Dictionary<string, int> _operatorPriority = new Dictionary<string, int> { { "^", 3 }, { "*", 2 }, { "/", 2 }, { "+", 1 }, { "-", 1 } };
        static string _patternSI = @"(\d+)\s*(rad|deg)";
        static string _patternNumber = @"(?:[^()]+|\((?:[^()]+|\([^()]\))\))";
        static string _patternFunction = $"(?<function>{string.Join("|", _functions)})";
        static string _patternArg = $"(?<arg>{_patternNumber}+)";
        static string _variables = $"(?<var>pi|e)";
        static string _pattern = $"^{_patternFunction}\\({_patternArg}\\)|{_variables}";
        static Dictionary<string, Func<double, double>> _FunctionExtensions = new Dictionary<string, Func<double, double>>();
        static Dictionary<string, Func<double, double, double>> _FunctionOperators = new Dictionary<string, Func<double, double, double>>();
        static double CalculateSimple(ref string strMath, ref Stack<double> values, ref Stack<string> operators)
        {
            var stringValue = "";
            var isGroup = false;
            var result = 0.0d;
            var loop = true;
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
                            stringValue = "";
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
                                stringValue = "";
                                var fact = 1;
                                for (var i = 2; i <= value; i++)
                                {
                                    fact *= i;
                                }
                                value = fact;
                                values.Push(value);
                            }
                            else
                            {
                                stringValue = "";
                                return double.NaN;
                            }
                        }
                        break;
                    case "%":
                        {
                            if (double.TryParse(stringValue, out var value))
                            {
                                stringValue = "";
                                value /= 100;
                                values.Push(value);
                            }
                            else
                            {
                                stringValue = "";
                                return double.NaN;
                            }
                        }
                        break;
                    default:
                        {
                            if (_operatorPriority.TryGetValue(op, out int value))
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
                //loop = calToEnd;
            }
            if (!string.IsNullOrEmpty(stringValue))
            {
                if (double.TryParse(stringValue, out var num))
                {
                    values.Push(num);
                }
                stringValue = "";
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
                    double value;
                    if (matchSI.Success)
                    {
                        functionValid = double.TryParse(matchSI.Groups[1].Value, out value);
                    }
                    else
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
                _ => 0,
            };
        }

    }
}
﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using VTNET.Extensions.Models;

namespace VTNET.Extensions.SupportFunctions
{
    public class TextDifferenceInfo
    {
        public List<DifferenceInfo> CurrentInfo { get; set; } = new List<DifferenceInfo>();
        public List<DifferenceInfo> DifferenceInfo { get; set; } = new List<DifferenceInfo>();
    }
    public struct DifferenceInfo
    {
        public char Character;
        public int Position;

        public DifferenceInfo(char character, int position)
        {
            Character = character;
            Position = position;
        }
    }

    public static class StringAnalysis
    {
        static readonly string patternFunctionParams = @"\(((?>[^()\\]+|\\(\(|\\(?<DEPTH>)|\\(\)|\\)(?<-DEPTH>)|\\)(?!\\)|\\.|[^()]+)*)\)";
        static readonly string patternFunctionCall = @"(\w+)\(((?>[^()\\]+|\\(\(|\\(?<DEPTH>)|\\(\)|\\)(?<-DEPTH>)|\\)(?!\\)|\\.|[^()]+)*)\)";
        static readonly string patternFunctionNoName = @"\(((?>[^()\\]+|\\(\(|\\(?<DEPTH>)|\\(\)|\\)(?<-DEPTH>)|\\)(?!\\)|\\.|[^()]+)*)\)\{((?>[^{}\\]+|\\(\(|\\(?<DEPTH>)|\\(\)|\\)(?<-DEPTH>)|\\)(?!\\)|\\.|[^{}]+)*)\}";
        static readonly string patternFunction = @"(\w+)" + patternFunctionNoName;
        static readonly string patternFunctionReplace = @"(\w+)\(((?>[^()]+|\((?<DEPTH>)|\)(?<-DEPTH>))*)(?(DEPTH)(?!))\)";
        public static List<string> FunctionParams(string? data, string prefix = "")
        {
            if (data == null)
            {
                return new List<string>();
            }
            List<string> dic = new List<string>();
            // Tìm tất cả các đối tượng phù hợp với pattern bằng Regex.Matches
            MatchCollection matches = Regex.Matches(data, prefix + patternFunctionParams);
            foreach (var item in from Match item in matches
                                 where item.Groups.Count > 1
                                 select item)
            {
                dic.Add(item.Groups[1].Value);
            }
            return dic;
        }
        public static Dictionary<string, string> FunctionsCall(string? data, string prefix = "")
        {
            if (data == null)
            {
                return new Dictionary<string, string>();
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            // Tìm tất cả các đối tượng phù hợp với pattern bằng Regex.Matches
            MatchCollection matches = Regex.Matches(data, prefix + patternFunctionCall);
            foreach (var item in from Match item in matches
                                 where item.Groups.Count > 2
                                 select item)
            {
                dic.Add(item.Groups[1].Value, item.Groups[2].Value);
            }
            return dic;
        }
        public static List<FunctionStructure> FunctionsCallAsList(string? data, string prefix = "")
        {
            if (data == null)
            {
                return new List<FunctionStructure>();
            }
            List<FunctionStructure> list = new List<FunctionStructure>();
            // Tìm tất cả các đối tượng phù hợp với pattern bằng Regex.Matches
            MatchCollection matches = Regex.Matches(data, prefix + patternFunction);
            foreach (var item in from Match item in matches
                                 where item.Groups.Count > 2
                                 select item)
            {
                list.Add(new FunctionStructure()
                {
                    OriginalString = item.Groups[0].Value,
                    FuncName = item.Groups[1].Value,
                    Param = item.Groups[2].Value
                });
            }
            return list;
        }
        public static List<FunctionStructure> Functions(string? data, string customName = "", string prefix = "")
        {
            if (data == null)
            {
                return new List<FunctionStructure>();
            }
            var pattern = customName.IsNullOrEmpty() ? patternFunction : $"({customName}){patternFunctionNoName}";
            List<FunctionStructure> list = new List<FunctionStructure>();
            MatchCollection matches = Regex.Matches(data, prefix + pattern);
            foreach (var item in from Match item in matches
                                 where item.Groups.Count > 4
                                 select item)
            {
                list.Add(new FunctionStructure()
                {
                    FuncName = item.Groups[1].Value,
                    Param = item.Groups[2].Value.Replace(@"\(", "(").Replace(@"\)", ")"),
                    Code = item.Groups[5].Value.Replace(@"\{", "{").Replace(@"\}", "}"),
                    OriginalString = item.Groups[0].Value
                });
            }
            return list;
        }

        /// <summary>
        /// using TwoLetterISOLanguageName, 
        /// template: @vi(tiếng việt)@en(English)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceByLanguage(string input, string prefix = "")
        {
            string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            string result = Regex.Replace(input, prefix+patternFunctionReplace, match =>
            {
                string matchedLang = match.Groups[1].Value;
                string replacement = match.Groups[2].Value;

                if (matchedLang == lang)
                {
                    return replacement;
                }
                else
                {
                    return ""; // Replace with an empty string for non-matching languages
                }
            });
            return result;
        }

        public static string ReplaceByFunc(string input, string funcName, Func<string, string> func, string prefix = "")
        {
            string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            string result = Regex.Replace(input, prefix+patternFunctionReplace, match =>
            {
                string matchedFuncName = match.Groups[1].Value;
                string replacement = match.Groups[2].Value;

                if (matchedFuncName == funcName)
                {
                    return func(replacement);
                }
                else if (matchedFuncName == lang)
                {
                    return replacement;
                }
                else
                {
                    return $"{matchedFuncName}({ReplaceByFunc(replacement, funcName, func)})";
                }
            });
            return result;
        }

        public static int LevenshteinDistance(string s1, string s2)
        {
            int[,] dp = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
            {
                for (int j = 0; j <= s2.Length; j++)
                {
                    if (i == 0)
                        dp[i, j] = j;
                    else if (j == 0)
                        dp[i, j] = i;
                    else
                        dp[i, j] = Math.Min(
                            Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                            dp[i - 1, j - 1] + (s1[i - 1] == s2[j - 1] ? 0 : 1)
                        );
                }
            }
            return dp[s1.Length, s2.Length];
        }

        public static TextDifferenceInfo GetDifferences(string s1, string s2)
        {
            TextDifferenceInfo differences = new TextDifferenceInfo();

            int[,] dp = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
            {
                for (int j = 0; j <= s2.Length; j++)
                {
                    if (i == 0)
                        dp[i, j] = j;
                    else if (j == 0)
                        dp[i, j] = i;
                    else
                        dp[i, j] = Math.Min(
                            Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                            dp[i - 1, j - 1] + (s1[i - 1] == s2[j - 1] ? 0 : 1)
                        );
                }
            }

            int m = s1.Length;
            int n = s2.Length;

            while (m > 0 || n > 0)
            {
                if (m > 0 && n > 0 && s1[m - 1] == s2[n - 1])
                {
                    m--;
                    n--;
                }
                else if (n > 0 && (m == 0 || dp[m, n - 1] <= dp[m - 1, n] && dp[m, n - 1] <= dp[m - 1, n - 1]))
                {
                    differences.DifferenceInfo.Add(new DifferenceInfo(s2[n - 1], m));
                    n--;
                }
                else
                {
                    differences.CurrentInfo.Add(new DifferenceInfo(s1[m - 1], m - 1));
                    m--;
                }
            }

            return differences;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using VTNET.Extensions.Models;

namespace VTNET.Extensions
{
    public class TextDifferenceInfo
    {
        public List<DifferenceInfo> InfoA { get; set; } = new List<DifferenceInfo>();
        public List<DifferenceInfo> InfoB { get; set; } = new List<DifferenceInfo>();
        [Obsolete("use InfoA")]
        public List<DifferenceInfo> CurrentInfo => InfoA;
        [Obsolete("use InfoB")]
        public List<DifferenceInfo> DifferenceInfo => InfoB;
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

    public static partial class StringAnalysis
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

        public static int LevenshteinDistance(string stringA, string stringB)
        {
            int[,] dp = new int[stringA.Length + 1, stringB.Length + 1];

            for (int i = 0; i <= stringA.Length; i++)
            {
                for (int j = 0; j <= stringB.Length; j++)
                {
                    if (i == 0)
                        dp[i, j] = j;
                    else if (j == 0)
                        dp[i, j] = i;
                    else
                        dp[i, j] = Math.Min(
                            Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                            dp[i - 1, j - 1] + (stringA[i - 1] == stringB[j - 1] ? 0 : 1)
                        );
                }
            }
            return dp[stringA.Length, stringB.Length];
        }

        public static TextDifferenceInfo GetDifferences(string stringA, string stringB)
        {
            TextDifferenceInfo differences = new();

            int[,] dp = new int[stringA.Length + 1, stringB.Length + 1];

            for (int i = 0; i <= stringA.Length; i++)
            {
                for (int j = 0; j <= stringB.Length; j++)
                {
                    if (i == 0)
                        dp[i, j] = j;
                    else if (j == 0)
                        dp[i, j] = i;
                    else
                        dp[i, j] = Math.Min(
                            Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                            dp[i - 1, j - 1] + (stringA[i - 1] == stringB[j - 1] ? 0 : 1)
                        );
                }
            }

            int m = stringA.Length;
            int n = stringB.Length;

            while (m > 0 || n > 0)
            {
                if (m > 0 && n > 0 && stringA[m - 1] == stringB[n - 1])
                {
                    m--;
                    n--;
                }
                else if (n > 0 && (m == 0 || dp[m, n - 1] <= dp[m - 1, n] && dp[m, n - 1] <= dp[m - 1, n - 1]))
                {
                    differences.InfoB.Add(new DifferenceInfo(stringB[n - 1], m));
                    n--;
                }
                else
                {
                    differences.InfoA.Add(new DifferenceInfo(stringA[m - 1], m - 1));
                    m--;
                }
            }

            return differences;
        }

        // Generates an array containing every two consecutive letters in the input string
        static string[] LetterPairs(string str)
        {
            int numPairs = str.Length - 1;
            string[] pairs = new string[numPairs];

            for (int i = 0; i < numPairs; i++)
            {
                pairs[i] = str.Substring(i, 2);
            }
            return pairs;
        }

        // Gets all letter pairs for each
        static List<string> WordLetterPairs(string str)
        {
            List<string> AllPairs = new();

            // Tokenize the string and put the tokens/words into an array
            string[] Words = Space().Split(str);

            // For each word
            for (int w = 0; w < Words.Length; w++)
            {
                if (!string.IsNullOrEmpty(Words[w]))
                {
                    // Find the pairs of characters
                    string[] PairsInWord = LetterPairs(Words[w]);

                    for (int p = 0; p < PairsInWord.Length; p++)
                    {
                        AllPairs.Add(PairsInWord[p]);
                    }
                }
            }
            return AllPairs;
        }

        /// <summary>
        /// Compares the two strings based on letter pair matches
        /// </summary>
        /// <returns>percentage</returns>
        public static double CompareStrings(string str1, string str2)
        {
            var pairs1 = WordLetterPairs(str1.ToUpper());
            var pairs2 = WordLetterPairs(str2.ToUpper());

            int intersection = 0;
            int union = pairs1.Count + pairs2.Count;

            for (int i = 0; i < pairs1.Count; i++)
            {
                for (int j = 0; j < pairs2.Count; j++)
                {
                    if (pairs1[i] == pairs2[j])
                    {
                        intersection++;
                        pairs2.RemoveAt(j);//Must remove the match to prevent "AAAA" from appearing to match "AA" with 100% success
                        break;
                    }
                }
            }

            return 2.0 * intersection * 100 / union; //returns in percentage
                                                       //return (2.0 * intersection) / union; //returns in score from 0 to 1
        }

        [GeneratedRegex("\\s")]
        private static partial Regex Space();
    }
}

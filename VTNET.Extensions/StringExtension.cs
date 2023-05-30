using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using VTNET.Extensions.Languages;

namespace VTNET.Extensions
{
    public static class StringExtension
    {
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
        public static void SetLanguageToWords(LanguageDefinition lang)
        {
            LanguageNumberWords.SetLanguageNumberWords(lang);
        }
        public static void SetLanguageToWords(
            string zero, string minus, string billion, string million, string thousand, string hundred,
            string unitZero, string unitOne, string unitTwo, string unitThree, string unitFour, string unitFive, string unitSix, string unitSeven, string unitEight, string unitNine, string unitTen, string unitEleven, string unitTwelve, string unitThirteen, string unitFourteen, string unitFifteen, string unitSixteen, string unitSeventeen, string unitEighteen, string unitNineteen,
            string tenZero, string tenTen, string tenTwenty, string tenThirty, string tenForty, string tenFifty, string tenSixty, string tenSeventy, string tenEighty, string tenNinety
            )
        {
            LanguageNumberWords.SetLanguageNumberWords(zero: zero,minus: minus,billion: billion,million: million,thousand: thousand,hundred: hundred,
                unitZero: unitZero,unitOne: unitOne,unitTwo: unitTwo,unitThree: unitThree,unitFour: unitFour,unitFive: unitFive,unitSix: unitSix,unitSeven: unitSeven,unitEight: unitEight,unitNine: unitNine,unitTen: unitTen,unitEleven: unitEleven,unitTwelve: unitTwelve,unitThirteen: unitThirteen,unitFourteen: unitFourteen,unitFifteen: unitFifteen,unitSixteen: unitSixteen,unitSeventeen: unitSeventeen,unitEighteen: unitEighteen,unitNineteen: unitNineteen,
                tenZero: tenZero,tenTen: tenTen,tenTwenty: tenTwenty,tenThirty: tenThirty,tenForty: tenForty,tenFifty: tenFifty,tenSixty: tenSixty,tenSeventy: tenSeventy,tenEighty: tenEighty,tenNinety: tenNinety);
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

    }
}

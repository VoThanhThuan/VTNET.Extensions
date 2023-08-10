using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using VTNET.Extensions.Languages;
using VTNET.Extensions.Model;

namespace VTNET.Extensions
{
    public static class StringEx
    {

        public static string Lorem { get => LoremIpsum();}
        public static string LoremShort { get => LoremIpsum(4, 16, 0, 1, 1);}
        public static string LoremLong { get => LoremIpsum(4, 64, 1, 4, 4);}

        public static bool Contains(this string? text, Func<OptionsContains, bool> options)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var optionsContains = new OptionsContains(text);

            return options(optionsContains);
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
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumericString(this string? input, char? thousandSeparator = null, char? decimalDigits = null)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            if (thousandSeparator != null)
            {
                input = input.Replace(thousandSeparator.ToString(), "");
            }
            if (decimalDigits != null)
            {
                input = input.Replace(decimalDigits.ToString(), ".");
            }
            string pattern = @"^-?\d*\.?\d+$";
            return Regex.IsMatch(input, pattern);
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
        public static string Separator(this double s, char separator = ',', char decimalPoint = '.', int? digits = null)
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
            if(decimalPoint != '.')
            {
                decimalSeparator = decimalPoint;
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
        public static string Separator(this float s, char separator = ',', char decimalPoint = '.', int? digits = null)
        {
            return Separator((double)s, separator, decimalPoint, digits);
        }

        /// <summary>
        /// Thousand separator for numbers
        /// </summary>
        public static string Separator(this int s, char separator = ',', char decimalPoint = '.', int? digits = null)
        {
            return Separator((float)s, separator, decimalPoint, digits);
        }

        /// <summary>
        /// Thousand separator for numbers
        /// </summary>
        public static string Separator(this long s, char separator = ',', char decimalPoint = '.', int? digits = null)
        {
            return Separator((double)s, separator, decimalPoint, digits);
        }

        /// <summary>
        /// Thousand separator for numbers
        /// </summary>
        public static string Separator(this string s, char separator = ',', char decimalPoint = '.', int? digits = null)
        {
            return Separator(double.Parse(s), separator, decimalPoint, digits);
        }

        public static void SetLanguageToWords(LangWords lang)
        {
            LanguageNumberWords.Language = lang;
        }

        public static string NumberToEnglishText(double inputNumber, bool includeCents = true)
        {
            string[] unitNumbers = new string[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            string[] tensNumbers = new string[] { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
            string[] teensNumbers = new string[] { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] placeValues = new string[] { "", "thousand", "million", "billion", "trillion" };
            bool isNegative = false;

            if (inputNumber < 0)
            {
                isNegative = true;
                inputNumber = Math.Abs(inputNumber);
            }

            long integerPart = (long)inputNumber;
            int decimalPart = (int)Math.Round((inputNumber - integerPart) * 100); // Lấy tối đa hai chữ số phần thập phân

            string result = "";

            if (integerPart == 0)
            {
                result = unitNumbers[0];
            }
            else
            {
                int placeValue = 0;

                while (integerPart > 0)
                {
                    int chunk = (int)(integerPart % 1000);
                    if (chunk > 0)
                    {
                        if (result != "")
                        {
                            result = " " + result; // Thêm khoảng trắng giữa các từ
                        }
                        result = ConvertChunkToWords(chunk, unitNumbers, tensNumbers, teensNumbers) + " " + placeValues[placeValue] + result;
                    }
                    integerPart /= 1000;
                    placeValue++;
                }
            }

            if (isNegative)
            {
                result = "negative " + result;
            }

            if (decimalPart > 0 && includeCents)
            {
                result += "and " + ConvertChunkToWords(decimalPart, unitNumbers, tensNumbers, teensNumbers) + " cents";
            }

            return result;
        }

        private static string ConvertChunkToWords(int chunk, string[] unitNumbers, string[] tensNumbers, string[] teensNumbers)
        {
            string chunkText = "";

            int hundreds = chunk / 100;
            int tens = (chunk % 100) / 10;
            int ones = chunk % 10;

            if (hundreds > 0)
            {
                chunkText += unitNumbers[hundreds] + " hundred";
                if (tens > 0 || ones > 0)
                {
                    chunkText += " and ";
                }
            }

            if (tens == 1)
            {
                chunkText += teensNumbers[ones];
            }
            else if (tens > 1)
            {
                chunkText += tensNumbers[tens - 2];
                if (ones > 0)
                {
                    chunkText += "-" + unitNumbers[ones];
                }
            }
            else if (ones > 0)
            {
                chunkText += unitNumbers[ones];
            }

            return chunkText;
        }


        /// <summary>
        /// Thousand separator for numbers
        /// </summary>
        public static string ToWords(this int number)
        {
            return ToWords(number, true, LangWords.DEFAULT);
        }

        /// <summary>
        /// Thousand separator for numbers
        /// </summary>
        public static string ToWords(this double inputNumber, bool suffix = true, LangWords lang = LangWords.DEFAULT)
        {
            if((lang == LangWords.DEFAULT && LanguageNumberWords.Language == LangWords.EN) || lang == LangWords.EN)
            {
                return NumberToEnglishText(inputNumber);
            }

            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"
            string sNumber = inputNumber.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            return result + (suffix ? " đồng" : "");
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
        public static string LoremIpsum(int minWords = 4, int maxWords = 16, int minSentences = 1, int maxSentences = 2, int numParagraphs = 4)
        {
            var words = new[] { "lorem", "ipsum", "dolor", "sit", "amet", 
                "consectetur", "adipisci", "elit", "sed", "diam", 
                "nonummy", "nibh", "euismod", "tincidunt", "ut", 
                "laoreet", "dolore", "ut", "enim", "ad", 
                "minim", "veniam", "quis", "nostrum", "exercitationem", 
                "ullam", "corporis", "suscipit", "laboriosam,", "nisi", 
                "ut", "aliquid", "ex", "ea", "commodi", 
                "consequatur", "quis", "aute", "iure", "reprehenderit", 
                "in", "voluptate", "velit", "esse", "cillum", 
                "dolore", "eu", "fugiat", "nulla", "pariatur", 
                "excepteur", "sint", "obcaecat", "cupiditat", 
                "non", "proident", "sunt", "in", "culpa", 
                "qui", "officia", "deserunt", "mollit", "anim", 
                "id", "est", "laborum", "vothuan" };

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
            var numSentences = rand.Next(maxSentences - minSentences) + minSentences + 1;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReverseString(this string input)
        {
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using VTNET.Extensions.Languages;
using static System.String;

namespace VTNET.Extensions.SupportFunctions
{
    public static class NumberWordsConverter
    {
        private static readonly ConversionFactory _conversionFactory = new ConversionFactory();

        /// <summary>
        /// Converts to words as per defined option
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToWordsEnglish(decimal number)
        {
            if (number < 0) return Empty;
            decimal fractionalDigits = number % 1;
            string integralDigitsString = number
                .ToString(CultureInfo.InvariantCulture)
                .Split('.')
                .ElementAt(0);
            string fractionalDigitsString = fractionalDigits.ToString($"F2",
                                                    CultureInfo.InvariantCulture)
                                                .Split('.')
                                                .ElementAtOrDefault(1) ?? Empty;
            if (decimal.Parse(integralDigitsString) < 0 && decimal.Parse(fractionalDigitsString) < 0) return Empty;

            string integralWords = Empty;
            if (decimal.Parse(integralDigitsString) > 0)
            {
                integralWords = _conversionFactory.ConvertDigits(integralDigitsString);
                //integralWords = integralWords;
            }

            if (int.Parse(fractionalDigitsString) < 0 || IsNullOrEmpty(fractionalDigitsString)) return integralWords;

            string fractionalWords = _conversionFactory.ConvertDigits(fractionalDigitsString);


            fractionalWords = (integralWords + $" {LanguageNumberWords.FloatPointEN} " + fractionalWords).Trim();

            return fractionalWords;

        }
        static string DetectZeroFloatNumber(string floatNumber, string unitNumbers)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < floatNumber.Length && floatNumber[i] == '0'; i++)
            {
                result.Append($"{unitNumbers} ");
            }
            return result.ToString();
        }
        public static string ToWordsVietnamese(decimal inputNumber)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            if(inputNumber == 0)
            {
                return unitNumbers[0];
            }

            // -12345678.3445435 => "-12345678"
            // Tạo một CultureInfo dựa trên ngôn ngữ hiện hành của hệ thống
            CultureInfo culture = CultureInfo.CurrentCulture;
            string decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
            var splitFloatNumber = inputNumber.ToString().Split(decimalSeparator.ToCharArray());
            string floatNumber = splitFloatNumber.Length > 1 ? splitFloatNumber[1] : "";
            string sNumber = splitFloatNumber[0] == "0" ? "" : splitFloatNumber[0];
            decimal number = sNumber.IsNullOrEmpty() ? 0 : Convert.ToDecimal(sNumber);
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
                        result = "mốt " + result;
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
            if (!IsNullOrEmpty(floatNumber))
            {
                result += $" {LanguageNumberWords.FloatPointVN} " + DetectZeroFloatNumber(floatNumber, unitNumbers[0]) + ToWordsVietnamese(decimal.Parse(floatNumber));
            }
            if (isNegative) result = "Âm " + result;
            return result;
        }

    }
}

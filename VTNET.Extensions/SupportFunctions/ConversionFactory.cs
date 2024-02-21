using System;
using System.Globalization;
using System.Text;
using static System.String;

namespace VTNET.Extensions.SupportFunctions
{
    public class ConversionFactory
    {
        public readonly string[] _ones = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
        public readonly string[] _tens = { "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
        public readonly string[] _scale = { "", "hundred", "thousand", "million", "billion", "trillion", "quadrillion", "quintillion", "sextillion", "septillion", "octillion", "nonillion", "decillion", "undecillion", "duodecillion " };

        internal string ToOnesWords(ushort digit) => _ones[digit];

        internal string ToTensWord(string tenth)
        {
            int dec = Convert.ToUInt16(tenth, CultureInfo.InvariantCulture);
            if (dec <= 0) return Empty;
            string words;

            if (dec < 20)
            {
                words = _ones[dec];
            }
            else
            {
                if (dec % 10 == 0)
                {
                    words = _tens[dec / 10];
                }
                else
                {
                    int first = Convert.ToUInt16(tenth.Substring(0, 1), CultureInfo.InvariantCulture);
                    int second = Convert.ToUInt16(tenth.Substring(1, 1), CultureInfo.InvariantCulture);
                    words = Concat(_tens[first], " ", _ones[second]);
                }
            }

            return words;
        }

        internal string ToHundredthWords(string hundred)
        {
            string inWords = Empty;
            if (hundred.Length == 3)
            {
                int hundredth = Convert.ToInt16(hundred.Substring(0, 1), CultureInfo.InvariantCulture);
                inWords = hundredth > 0 ? Concat(_ones[hundredth], " ", _scale[1], " ") : Empty;
                hundred = hundred.Substring(1, 2);
            }
            inWords += ToTensWord(hundred);
            return inWords.Trim();
        }

        /// <summary>
        /// Responsible for converting any input digits to words
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        internal string ConvertDigits(string digits)
        {
            if (digits == "0" || digits == "00") return _ones[0];
            StringBuilder builder = new StringBuilder();
            int scaleMapIndex = (int)Math.Ceiling((decimal)digits.Length / 3);
            for (int i = scaleMapIndex; i > 0; i--)
            {
                string inWords;
                switch (i)
                {
                    case 1: //For the Hundreds, tens and ones
                        inWords = ToHundredthWords(digits);
                        if (!IsNullOrEmpty(inWords))
                            builder.Append(Concat(inWords.Trim(), " "));
                        break;
                    default: //For Everything Greater than hundreds
                        int length = (digits.Length % ((i - 1) * 3 + 1)) + 1;
                        string hundreds = digits.Substring(0, length);
                        digits = digits.Remove(0, length);
                        inWords = ToHundredthWords(hundreds);

                        if (!IsNullOrEmpty(inWords.Trim()))
                            builder.Append(Concat(inWords.Trim(), " ", _scale[i], " "));
                        break;
                }
            }
            return builder.ToString().Trim();
        }

    }
}

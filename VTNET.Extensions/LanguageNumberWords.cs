using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTNET.Extensions.Languages
{
    public enum LanguageDefinition
    {
        VN,EN
    }
    public static class LanguageNumberWords
    {
        static Dictionary<string, string> LanguageVN { get; } = new Dictionary<string, string>() {
                { "Zero", "Không"},
                { "Minus",  "Âm"},
                { "Billion",  "tỉ"},
                { "Million", "triệu"},
                { "Thousand", "nghìn"},
                { "Hundred", "trăm"},

                { "UnitZero", "không"},
                { "UnitOne", "một"},
                { "UnitTwo", "hai"},
                { "UnitThree", "ba"},
                { "UnitFour", "bốn"},
                { "UnitFive", "năm"},
                { "UnitSix", "sáu"},
                { "UnitSeven", "bảy"},
                { "UnitEight", "tám"},
                { "UnitNine", "chín"},
                { "UnitTen", "mười"},
                { "UnitEleven", "mười một"},
                { "UnitTwelve", "mười hai"},
                { "UnitThirteen", "mười ba"},
                { "UnitFourteen", "mười bốn"},
                { "UnitFifteen", "mười lăm"},
                { "UnitSixteen", "mười sáu"},
                { "UnitSeventeen", "mười bảy"},
                { "UnitEighteen", "mười tám"},
                { "UnitNineteen", "mười chín"},

                { "TenZero", "không"},
                { "TenTen",  "mười"},
                { "TenTwenty",  "hai mươi"},
                { "TenThirty", "ba mươi"},
                { "TenForty", "bốn mươi"},
                { "TenFifty", "năm mươi"},
                { "TenSixty", "sáu mươi"},
                { "TenSeventy", "bảy mươi"},
                { "TenEighty", "tám mươi"},
                { "TenNinety", "chín mươi"},
        };
        static Dictionary<string, string> LanguageEN { get; } = new Dictionary<string, string>() {
            { "Zero", "Zero" },
            { "Minus", "Minus" },
            { "Billion", "billion" },
            { "Million", "million" },
            { "Thousand", "thousand" },
            { "Hundred", "hundred" },

            { "UnitZero", "zero" },
            { "UnitOne", "one" },
            { "UnitTwo", "two" },
            { "UnitThree", "three" },
            { "UnitFour", "four" },
            { "UnitFive", "five" },
            { "UnitSix", "six" },
            { "UnitSeven", "seven" },
            { "UnitEight", "eight" },
            { "UnitNine", "nine" },
            { "UnitTen", "ten" },
            { "UnitEleven", "eleven" },
            { "UnitTwelve", "twelve" },
            { "UnitThirteen", "thirteen" },
            { "UnitFourteen", "fourteen" },
            { "UnitFifteen", "fifteen" },
            { "UnitSixteen", "sixteen" },
            { "UnitSeventeen", "seventeen" },
            { "UnitEighteen", "eighteen" },
            { "UnitNineteen", "nineteen" },

            { "TenZero", "zero" },
            { "TenTen", "ten" },
            { "TenTwenty", "twenty" },
            { "TenThirty", "thirty" },
            { "TenForty", "forty" },
            { "TenFifty", "fifty" },
            { "TenSixty", "sixty" },
            { "TenSeventy", "seventy" },
            { "TenEighty", "eighty" },
            { "TenNinety", "ninety" },
        };

        static Dictionary<string, string> LanguageDefault { get; set; } = LanguageEN;
        public static Dictionary<string, string> Language { get => LanguageDefault; }
        public static string[] UnitsMap { get => new string[]{
                LanguageDefault["UnitZero"],
                LanguageDefault["UnitOne"],
                LanguageDefault["UnitTwo"],
                LanguageDefault["UnitThree"],
                LanguageDefault["UnitFour"],
                LanguageDefault["UnitFive"],
                LanguageDefault["UnitSix"],
                LanguageDefault["UnitSeven"],
                LanguageDefault["UnitEight"],
                LanguageDefault["UnitNine"],
                LanguageDefault["UnitTen"],
                LanguageDefault["UnitEleven"],
                LanguageDefault["UnitTwelve"],
                LanguageDefault["UnitThirteen"],
                LanguageDefault["UnitFourteen"],
                LanguageDefault["UnitFifteen"],
                LanguageDefault["UnitSixteen"],
                LanguageDefault["UnitSeventeen"],
                LanguageDefault["UnitEighteen"],
                LanguageDefault["UnitNineteen"]            
            }; 
        }
        public static string[] TensMap {
            get => new string[]{
                LanguageDefault["TenZero"],
                LanguageDefault["TenTen"],
                LanguageDefault["TenTwenty"],
                LanguageDefault["TenThirty"],
                LanguageDefault["TenForty"],
                LanguageDefault["TenFifty"],
                LanguageDefault["TenSixty"],
                LanguageDefault["TenSeventy"],
                LanguageDefault["TenEighty"],
                LanguageDefault["TenNinety"]
            };
        }

        public static void SetLanguageNumberWords(LanguageDefinition lang)
        {
            if (lang == LanguageDefinition.EN)
            {
                LanguageDefault = LanguageEN;
            }
            else
            {
                LanguageDefault = LanguageVN;
            }
        }

        public static void SetLanguageNumberWords(
            string zero, string minus, string billion, string million, string thousand, string hundred,
            string unitZero, string unitOne, string unitTwo, string unitThree, string unitFour, string unitFive, string unitSix, string unitSeven, string unitEight, string unitNine, string unitTen, string unitEleven, string unitTwelve, string unitThirteen, string unitFourteen, string unitFifteen, string unitSixteen, string unitSeventeen, string unitEighteen, string unitNineteen,
            string tenZero, string tenTen, string tenTwenty, string tenThirty, string tenForty, string tenFifty, string tenSixty, string tenSeventy, string tenEighty, string tenNinety
            )
        {
            LanguageDefault = new Dictionary<string, string>()
            {
                { "Zero", zero},
                { "Minus", minus},
                { "Billion", billion},
                { "Million", million},
                { "Thousand", thousand},
                { "Hundred", hundred},

                { "UnitZero", unitZero},
                { "UnitOne", unitOne},
                { "UnitTwo", unitTwo},
                { "UnitThree", unitThree},
                { "UnitFour", unitFour},
                { "UnitFive", unitFive},
                { "UnitSix", unitSix},
                { "UnitSeven", unitSeven},
                { "UnitEight", unitEight},
                { "UnitNine", unitNine},
                { "UnitTen", unitTen},
                { "UnitEleven", unitEleven},
                { "UnitTwelve", unitTwelve},
                { "UnitThirteen", unitThirteen},
                { "UnitFourteen", unitFourteen},
                { "UnitFifteen", unitFifteen},
                { "UnitSixteen", unitSixteen},
                { "UnitSeventeen", unitSeventeen},
                { "UnitEighteen", unitEighteen},
                { "UnitNineteen", unitNineteen},

                { "tenZero", tenZero},
                { "tenTen", tenTen},
                { "tenTwenty", tenTwenty},
                { "tenThirty", tenThirty},
                { "tenForty", tenForty},
                { "tenFifty", tenFifty},
                { "tenSixty", tenSixty},
                { "tenSeventy", tenSeventy},
                { "tenEighty", tenEighty},
                { "tenNinety", tenNinety}
            };
        }
    }
}

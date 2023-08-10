using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTNET.Extensions.Languages
{
    public enum LangWords
    {
        DEFAULT,VN,EN,DIFFERENT
    }
    public static class LanguageNumberWords
    {
        public static LangWords Language { get; set; } = LangWords.EN;

        static string _floatPointEN = "point";
        static string _floatPointVN = "phẩy";
        public static string FloatPointVN => _floatPointVN;
        public static string FloatPointEN => _floatPointEN;
        public static void SetFloatPoint(LangWords lang, string words)
        {
            if (string.IsNullOrEmpty(words))
            {
                return;
            }
            if(lang == LangWords.VN)
            {
                _floatPointVN = words;
            }
            else
            {
                _floatPointEN = words;
            }
        }
    }
}

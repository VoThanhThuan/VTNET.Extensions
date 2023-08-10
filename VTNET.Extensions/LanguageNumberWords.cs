using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTNET.Extensions.Languages
{
    public enum LangWords
    {
        DEFAULT,VN,EN
    }
    public static class LanguageNumberWords
    {
        public static LangWords Language { get; set; } = LangWords.EN;
    }
}

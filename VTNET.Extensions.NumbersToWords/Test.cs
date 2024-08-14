using System;
using System.Collections.Generic;
using System.Text;
using VTNET.Extensions;

namespace VTNET.Extensions.NumbersToWords
{
    internal class Test
    {
        public void test()
        {
            var textEN = NumberWordsConverter.ToWordsEnglish(10000);
            var textVI = NumberWordsConverter.ToWordsVietnamese(10000);
        }
    }
}

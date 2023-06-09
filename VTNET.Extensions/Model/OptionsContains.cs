using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using VTNET.Extensions.EnumDefinition;

namespace VTNET.Extensions.Model
{
    public class OptionsContains
    {
        private readonly string text;

        public OptionsContains(string text)
        {
            this.text = text;
        }   

        // Thêm các phương thức options khác tại đây (nếu cần)
        public bool Number { get => Regex.IsMatch(text, @"\d"); }
        public bool NumberOnly { get => Regex.IsMatch(text, @"^\d+$"); }
        public bool Text { get => Regex.IsMatch(text, @"\D"); }
        public bool TextOnly { get => Regex.IsMatch(text, @"^\D+$"); }
    }
}

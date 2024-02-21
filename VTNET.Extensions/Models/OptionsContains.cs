using System.Text.RegularExpressions;

namespace VTNET.Extensions.Models
{
    public partial class OptionsContains
    {
        private readonly string text;

        public OptionsContains(string text)
        {
            this.text = text;
        }   

        // Thêm các phương thức options khác tại đây (nếu cần)
        public bool Number { get => RegexNumber().IsMatch(text); }
        public bool NumberOnly { get => RegexNumberOnly().IsMatch(text); }
        public bool Text { get => RegexText().IsMatch(text); }
        public bool TextOnly { get => RegexTextOnly().IsMatch(text); }

        [GeneratedRegex("\\d")]
        private static partial Regex RegexNumber();
        [GeneratedRegex("^\\d+$")]
        private static partial Regex RegexNumberOnly();
        [GeneratedRegex("\\D")]
        private static partial Regex RegexText();
        [GeneratedRegex("^\\D+$")]
        private static partial Regex RegexTextOnly();
    }
}

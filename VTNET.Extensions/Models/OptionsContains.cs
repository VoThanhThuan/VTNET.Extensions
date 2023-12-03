using System.Text.RegularExpressions;

namespace VTNET.Extensions.Models
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

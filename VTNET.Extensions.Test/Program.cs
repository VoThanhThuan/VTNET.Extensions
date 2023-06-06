using System.Data;
using System.Text;
using VTNET.Extensions;
using VTNET.Extensions.Languages;

BooleanExtension.IsNumericString("-3.14").Log();
BooleanExtension.IsNumericString("1,000,000.34", ',').Log();
StringExtension.LoremIpsum(minWords: 4, maxWords: 64, minSentences: 1, maxSentences: 4, numParagraphs: 4);
"Thuaanj".ReverseString().Log();

var table = new DataTable();
table.Columns.Add("a");
table.Columns.Add("b");
table.Columns.Add("Z");
table.Rows.Add(new object[] { 1, 2, 3 });

var listTable2 = table.ToList<TestTable>();

Console.OutputEncoding = Encoding.UTF8;

StringExtension.SetLanguageToWords(LanguageDefinition.VN);

1000.ToWords().Log();

enum Test
{
    a,b,c,d,e,f,g,h
}

class TestTable
{
    public int a { get; set; }
    public int b { get; set; }
    [ColumnName("Z")]
    public int c { get; set; }
}
using System.Data;
using System.Text;
using VTNET.Extensions;
using VTNET.Extensions.Languages;

var a = "abc".Contains(x => x.TextOnly);
var b = "123abc".Contains(x => x.TextOnly);
var c = "abc".Contains(x => x.Number);
var d = "123abc".Contains(x => x.NumberOnly);
var e = "123abc".Contains(x => x.Number);
"".Log(a);
//"".FirstOrDefault(x => x == 'a');
//StringExtension.LoremIpsum(minWords: 4, maxWords: 64, minSentences: 1, maxSentences: 4, numParagraphs: 4);
//"Thuaanj".ReverseString().Log();

//var table = new DataTable();
//table.Columns.Add("a");
//table.Columns.Add("b");
//table.Columns.Add("Z");
//table.Rows.Add(new object[] { 1, 2, 3 });

//var listTable2 = table.ToList<TestTable>();

//Console.OutputEncoding = Encoding.UTF8;

//StringExtension.SetLanguageToWords(LanguageDefinition.VN);

//1000.ToWords().Log();

//enum Test
//{
//    a,b,c,d,e,f,g,h
//}

//class TestTable
//{
//    public int a { get; set; }
//    public int b { get; set; }
//    [ColumnName("Z")]
//    public int c { get; set; }
//}
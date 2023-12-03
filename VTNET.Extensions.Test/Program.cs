using System.Globalization;
using System.Text;
using VTNET.Extensions;
using VTNET.Extensions.Languages;
using VTNET.Extensions.SupportFunctions;

Console.OutputEncoding = Encoding.UTF8;
StringEx.SetLanguageToWords(LangWords.VN);

//var testToTitle = "NGAY lương".ToTitle(ignoreUpperCase:true);

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("vi-VN");

var function = StringAnalysis.Functions("testfunc(this is param){this is code}");//{FuncName: testfunc, Param: this is param, Code: this is code}
var functionCall = StringAnalysis.FunctionsCall("testfunc1(param one)testfunc2(param two)");//[(testfunc1,param one), (testfunc2,param two)]
var functionParams = StringAnalysis.FunctionParams("(param one)(param two)(param three)");//[param one, param two,param three]
var replaceByLang = StringAnalysis.ReplaceByLanguage("vi(Tiếng Việt)en(Tiếng anh)");
var replaceByFunc = StringAnalysis.ReplaceByFunc("the result of the calculation 3*6 is calc(3*6)","calc", data =>
{
    return data.Calculate().ToString();
}); //the result of the calculation 3*6 is 18
Console.WriteLine();
//var tes = 21.ToWords();
//tes = 321.ToWords();
//tes = 4321.ToWords();
//tes = 54321.ToWords();
//tes = 654321.ToWords();
//tes = 654312.ToWords();
//tes = 654132.ToWords();
//tes = 651432.ToWords();
//tes = 615432.ToWords();
//tes = 165432.ToWords();
//Console.WriteLine(tes);
//var a = 5;
//var b = "a";
//int? c = 1;
//string? d = "0";
//NumberEx.IsNumberType(a).Log();
//NumberEx.IsNumberType(b).Log();
//NumberEx.IsNumberType(c).Log();
//StringEx.IsNumeric(d).Log();

//var dic = new List<Dictionary<string, object?>>
//        {
//            new() { { "ID", 1 }, { "Name", "John" }, { "Age", 30 } },
//            new() { { "ID", 2 }, { "Name", "Alice" }, { "Age", 25 } },
//            new() { { "ID", 3 }, { "Name", "Bob" }, { "Age", null } }
//        };
//var table = dic.ToDataTable();
//var valueMap = table.ToList<TestTable>();

//while (true)
//{
//    Console.WriteLine("Nhập số tiền: ");
//    var amount = int.Parse(Console.ReadLine());

//    string amountInWords = amount.ToWords();
//    string amountInWordsEN = amount.ToWords(LangWords.EN);
//    Console.WriteLine("Số tiền bằng chữ (tiếng việt): " + amountInWords);
//    Console.WriteLine("Số tiền bằng chữ (tiếng anh): " + amountInWordsEN);
//}

//var list = new List<int>() { 
//    1, 2, 2, 2,
//    1, 3, 3, 3, 3,
//    1, 4, 4, 4, 4,
//    1, 5, 5, 5, 5
//};
//var b = list.Split(1);
//b.Log();
//1000.Separator();
//var a = "abc".Contains(x => x.Text);
//var b = "123abc".Contains(x => x.TextOnly);
//var c = "abc".Contains(x => x.Number);
//var d = "123abc".Contains(x => x.NumberOnly);
//"".Log(a);
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

class TestTable
{
    [MapColumnName("Id")]
    public string Idx { get; set; } = "";
    [IgnoreMapColumnName]
    public string Name { get; set; } = "";
    public string Age { get; set; } = "";
}
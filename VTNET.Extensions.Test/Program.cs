using System.Text;
using System.Text.Json;
using VTNET.Extensions;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

//var a = new Params
//{
//    {"a", 1 },
//    { "b", 2 },
//    { "c", 3 },
//    { 4 },
//    { 5 },
//    { 6 },
//};
//var json = JsonSerializer.Serialize(a);
//Console.WriteLine(json);
//CalculateEx.AddVariable("không", 0);
//CalculateEx.AddVariable("một", 1);
//CalculateEx.AddVariable("hai", 2);
//CalculateEx.AddVariable("ba", 3);
//CalculateEx.AddVariable("bốn", 4);
//CalculateEx.AddVariable("năm", 5);
//CalculateEx.AddVariable("sáu", 6);
//CalculateEx.AddVariable("bảy", 7);
//CalculateEx.AddVariable("tám", 8);
//CalculateEx.AddVariable("chín", 9);
//CalculateEx.AddFunction("cộng", numbers =>
//{
//    return numbers.Sum();
//});
//CalculateEx.AddFunction("xxx", numbers =>
//{
//    return numbers[0] * numbers[1] * numbers[2];
//});
//while (true)
//{
//    Console.WriteLine("Nhập phép tính");
//    Console.Write("> ");
//    var text = Console.ReadLine();
//    if (!text.IsNullOrEmpty())
//    {
//        text = text.Replace("cộng", "+", StringComparison.CurrentCultureIgnoreCase);
//        Console.Write("= ");
//        var result = text.Calculate();
//        Console.WriteLine(result);
//    }
//}

//var p = new ParamValue<int>(1);
//var p1 = new KeyValuePair<string, int>("", 1);
//Params<int> a = [1, 2, 3, 4, 5];
//var a1 = a.Select(x => x.ToString());
//var t = new TestTable();
//Params b = [("0", 1), ("b", 2), ("c", t), 1 , "2", 4.0d];
//var b1 = b.Select(x => x.ToString());
//Params d = [1, 2, 3, 4, "1"];
//var c = a.Get(1);


//System.Linq.Enumerable.Select(b, x => x.ToString());

////StringEx.SetLanguageToWords(LangWords.VN);
//CalculateEx.AddVariable("mot", 1);
//CalculateEx.AddVariable("hai", 2);
//CalculateEx.AddVariable("ba", 3);
//CalculateEx.AddVariable("bay", 7);
//var result = "mot + hai * (ba + bay)".Calculate();

var testTable = new TestTable()
{
    Idx = "123456789",
    Name = "BTestabc",
    Age = "69",
    Child = new()
    {
        Age = 24
    }
};

var v = ClassEx.Validation(testTable)
    .Check(x => x.Idx).Not.IsTextOnly("Idx chỉ nên là số")
    .Check(x => x.Name).Not.Contains(" ", "Name không nên chứa khoảng trắng")
        .StartsWith("A", "Name nên bắt đầu bằng A hoặc B")
        .Or
        .StartsWith("B", "Name nên bắt đầu bằng B")
    .Check(x => x.Child.Age).Number(max: 23, message: "Tuổi tối đa là 23");

var b = v.IsValid;
var c = v.Errors;
//var testClone = testTable.Map<TestMap>();
//testClone.Ten = "hahaha";
//CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("vi-VN");
//var test = double.Parse("1.234,56");
//var levenshtein = StringAnalysis.GetDifferences("võ thành thuận", "võ thành Thuặn");

//var function = StringAnalysis.Functions("testfunc(this is param){this is code}");//{FuncName: testfunc, Param: this is param, Code: this is code}
//var functionCall = StringAnalysis.FunctionsCall("testfunc1(param one)testfunc2(param two)");//[(testfunc1,param one), (testfunc2,param two)]
//var functionParams = StringAnalysis.FunctionParams("(param one)(param two)(param three)");//[param one, param two,param three]
//var replaceByLang = StringAnalysis.ReplaceByLanguage("vi(Tiếng Việt)en(Tiếng anh)");
//var replaceByFunc = StringAnalysis.ReplaceByFunc("the result of the calculation 3*6 is calc(3*6)","calc", data =>
//{
//    return data.Calculate().ToString();
//}); //the result of the calculation 3*6 is 18
//Console.WriteLine();
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
//var valueCast = table.Rows[0].Cast<TestTable>();
//var valueMap = table.ToList<TestTable>();
//var valueMap1 = table.ToListWithActivator<TestTable>();
//var valueMap2 = table.ToListParallel<TestTable>();
//var valueMap3 = table.ToListCache<TestTable>();
//var a = "";
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
    public string Name { get; set; } = "";
    public string Age { get; set; } = "";
    public TestMap Child { get; set; } = new();
}
class TestMap
{
    [MapColumnName("Id")]
    public string Ma { get; set; } = "";
    [MapName("Name")]
    public string Ten { get; set; } = "";
    public int Age { get; set; } 
}
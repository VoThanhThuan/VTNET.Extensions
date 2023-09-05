﻿using System.Data;
using System.Text;
using VTNET.Extensions;
using VTNET.Extensions.Languages;


Console.OutputEncoding = Encoding.UTF8;
StringEx.SetLanguageToWords(LangWords.VN);

var tes = 5.0.ToWords();
var a = 5;
var b = "a";
int? c = 1;
string? d = "0";
NumberEx.IsNumberType(a).Log();
NumberEx.IsNumberType(b).Log();
NumberEx.IsNumberType(c).Log();
StringEx.IsNumericString(d).Log();

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

//class TestTable
//{
//    public int a { get; set; }
//    public int b { get; set; }
//    [ColumnName("Z")]
//    public int c { get; set; }
//}
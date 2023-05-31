using System.Data;
using System.Text;
using VTNET.Extensions;
using VTNET.Extensions.Languages;
using VTNET.Extensions.Model;

var table = new DataTable();
table.Columns.Add("a");
table.Columns.Add("b");
table.Columns.Add("Z");
table.Rows.Add(new object[] { 1, 2, 3 });

var listTable2 = table.ToList<TestTable>();

Console.OutputEncoding = Encoding.UTF8;

StringExtension.SetLanguageToWords(LanguageDefinition.VN);

1000.ToWords().Log();

CalculateExtension.AddSimpleFunction("addone", num =>
{
    return ++num;
});

CalculateExtension.AddFunction("sum", agrs =>
{
    return agrs.Sum();
});

CalculateExtension.AddOperator('?', (a, b) =>
{
    return Random.Shared.Next((int)a, (int)b);
}, 3);

CalculateExtension.AddOperator('#', Math.Max, 3);


"addone(1)> ".Log("addone(1)".Calculate());
"1?100> ".Log("1?100".Calculate());
"sum(a,2,3,4,5,6)> ".Log("sum(a,2,3,4,5,6)".Calculate());
"1#2#3#6#5#4> ".Log("1#2#3#6#5#4".Calculate());

"sin(30deg)> ".Log("sin(30deg)".Calculate());
"tan(30deg)> ".Log("tan(30deg)".Calculate());
"cos(30deg)> ".Log("cos(30deg)".Calculate());
"log(30deg)> ".Log("log(30deg)".Calculate());

var a = new Dictionary<string, string> { { "a", "1" } };
var b = "1";
var c = new List<string>();
var d = new string[] { "1", "2" };
var e = Test.a;
var f = Test.b;
DateTime? g = null; 
DateTime? h = DateTime.Now; 
">> IsTrue:".Log();
"a> ".Log(a.IsTrue());
"b> ".Log(b.IsTrue());
"c> ".Log(c.IsTrue());
"d> ".Log(d.IsTrue());
"e> ".Log(e.IsTrue());
"f> ".Log(f.IsTrue());
"g> ".Log(g.IsTrue());
"h> ".Log(h.IsTrue());
Guid.NewGuid().IsTrue().Log();
"--------------------".Log();
">> IsFalse:".Log();
"a> ".Log(a.IsFalse());
"b> ".Log(b.IsFalse());
"c> ".Log(c.IsFalse());
"d> ".Log(d.IsFalse());
"e> ".Log(e.IsFalse());
"f> ".Log(f.IsFalse());
Guid.Empty.IsFalse().Log();
"--------------------".Log();
">> Calculate:".Log();
"sin(30)> ".Log("sin(30)".Calculate());
"tan(30)> ".Log("tan(30)".Calculate());
"cos(30)> ".Log("cos(30)".Calculate());
"log(30)> ".Log("log(30)".Calculate());




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
using VTNET.Extensions;
using VTNET.Extensions.Languages;

LanguageNumberWords.SetLanguageNumberWords(LanguageDefinition.VN);

1000.ToWords().Log();

"sin(30deg)".Log("sin(30deg)> ".Calculate());
"tan(30deg)".Log("tan(30deg)> ".Calculate());
"cos(30deg)".Log("cos(30deg)> ".Calculate());
"log(30deg)".Log("log(30deg)> ".Calculate());

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
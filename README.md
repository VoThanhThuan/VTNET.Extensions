# Library Name
Library Name is a collection of utility methods for manipulating various data types.

## Features
- Check if a string is a number: `"1000".IsNumber()`
- Check if a number is even: `69.IsEven()`
- Check if a number is odd: `96.IsOdd()`
- Convert a number to words: `"1000".ToWords()`
- Format a number as currency: `1000.ToCurrency()`
- Remove spaces from a string: `"a   b       c".RemoveDuplicateSpaces()`
- Capitalize the first letter of each word in a string: `"vo thanh thuan".Capitalize()`
- Convert a string to title case: `"vo thanh thuan".Title()`
- Convert a string to title case: `"".Lorem()`
- Convert DataTable to List: `var list = dataTable.ToList<model>()`
- Support string methods: `IsNullOrEmpty(), IsNullOrWhiteSpace()`
- Perform calculations on a string expression: `"1+1".Calculate()`
	- sin: `"sin(30)".Calculate()`
	- tan: `"tan(30)".Calculate()`
	- cos: `"cos(30)".Calculate()`
	- log: `"log(30)".Calculate()`
	- factorial: `"4!".Caculate()`
	- percent: `"44%".Caculate()`
## Installation

You can install the library via NuGet Package Manager. Simply search for `Library Name` and install the latest version.

## Usage

Here is an example of how to use the library:

```csharp
using VTNET.Extensions;

string str = "";
bool isEmpty = str.IsNullOrEmpty(); //true

string str = " ";
bool isEmpty = str.IsNullOrWhiteSpace(); //true

string number = "1000";
bool isNumber = number.IsNumber(); // true

int num = 69;
bool isEven = num.IsEven(); // false

int num2 = 96;
bool isOdd = num2.IsOdd(); // true

string number2 = "1000";
string words = number2.ToWords(); // "one thousand"

int amount = 1000;
string formattedAmount = amount.ToCurrency(); // "1,000.00"

string text = "a   b       c";
string trimmedText = text.RemoveDuplicateSpaces(); // "a b c"

string name = "vo thanh thuan";
string capitalized = name.Capitalize(); // "Vo thanh thuan"

string title = "vo thanh thuan";
string titleCase = title.Title(); // "Vo Thanh Thuan"

string expression = "1+1";
int result = expression.Calculate(); // 2

string titleCase = "".Lorem(); // "lorem ipsum dolor sit"

"Hello".Log(); //like Console.WriteLine("Hello");

",".Join(listValue); //like string.Join(",", listValue);

//Convert DataTable to List
var list = dataTable.ToList<model>();

```

### Calculate
```csharp
"sin(30)".Calculate(); //sin(30)> -0.9880316240928618
"sin(30deg)".Calculate(); //sin(30deg)> 0.49999999999999994

"tan(30)".Calculate(); //tan(30)> -6.405331196646276
"tan(30deg)".Calculate(); //tan(30deg)> 0.5773502691896257

"cos(30)".Calculate(); //cos(30)> 0.15425144988758405
"cos(30deg)".Calculate(); //cos(30deg)>0.8660254037844387

"log(30)".Calculate(); //log(30)> 1.4771212547196624

"4!".Calculate(); //4!> 44
"44%".Calculate(); //44%> 0.44

//Custom function
CalculateExtension.AddFunction("addOne", (double num) =>
{
    return ++num;
});
CalculateExtension.AddOperator('?', (double a, double b) =>
{
    return Random.Shared.Next((int)a, (int)b);
},priority: 3);

"addOne(1)> ".Log("addOne(1)".Calculate()); //addOne(1)> 2
"1?100> ".Log("1?100".Calculate());
```

### Convert a number to words:
```csharp
1000.ToWords(); // "one thousand"

//Set Language
Console.OutputEncoding = Encoding.UTF8;

StringExtension.SetLanguageToWords(LanguageDefinition.VN);

1000.ToWords(); // "một nghìn"
```

```csharp
```

### Check True, False
```csharp
enum Test{a,b,c,d,e,f,g,h}
var a = new Dictionary<string, string>{ { "a", "1"} };
var b = "1";
var c = new List<string>();
var d = new string[]{"1", "2"};
var e = Test.a;
var f = Test.b;
">> IsTrue:".Log();
"a> ".Log(a.IsTrue()); //true
"b> ".Log(b.IsTrue()); //true
"c> ".Log(c.IsTrue()); //false
"d> ".Log(d.IsTrue()); //true
"e> ".Log(e.IsTrue()); //false
"f> ".Log(f.IsTrue()); //true
"--------------------".Log();
">> IsFalse:".Log();
"a> ".Log(a.IsFalse()); //a> false
"b> ".Log(b.IsFalse()); //b> false
"c> ".Log(c.IsFalse()); //c> true
"d> ".Log(d.IsFalse()); //d> false
"e> ".Log(e.IsFalse()); //e> true
"f> ".Log(f.IsFalse()); //f> false
```

### DataTable To List
```csharp
var list = dataTable.ToList<model>();

//Match column name
var list2 = dataTable.ToList<model>(matchCase: true);

//Convert with cache
var list2 = dataTable.ToListCache<model>(matchCase: true);


//Specify the column name other fields
class TestTable{
	[ColumnName("IDX")]
	public string Id { get; set; } = "";
	[ColumnName("FULLNAME")]
	public string Name { get; set; } = "";
}
```
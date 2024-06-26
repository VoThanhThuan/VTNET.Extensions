﻿# Library Name
Library Name is a collection of utility methods for manipulating various data types.

## Features
- Check if a string is a number: `"1000".IsNumber()`
- Check if a number is even: `69.IsEven()`
- Check if a number is odd: `96.IsOdd()`
- Convert a number to words: `"1000".ToWords()`
- Format a number as currency: `1000.ToCurrency()`
- Remove spaces from a string: `"a   b       c".RemoveDuplicateSpaces()`
- Capitalize the first letter of each word in a string: `"vo thanh thuan".ToCapitalize()`
- Convert a string to title case: `"vo thanh thuan".ToTitle()`
- Convert a string to title case: `StringEx.Lorem`
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
"Hello".Log(); //Hello
```

### String
```csharp
string number2 = "1000";
string words = number2.ToWords(); // "one thousand"

int amount = 1000;
string formattedAmount = amount.Separator(); // "1,000.00"

string text = "a   b       c";
string trimmedText = text.RemoveDuplicateSpaces(); // "a b c"

string name = "vo thanh thuan";
string capitalized = name.ToCapitalize(); // "Vo thanh thuan"

string title = "VO THANH THUAN";
string titleCase1 = title.ToTitle(ignoreUpperCase:true); // "VO THANH THUAN"
string titleCase2 = title.ToTitle(ignoreUpperCase:false); // "Vo Thanh Thuan"

string reverseString = "Thuaanj".ReverseString(); // "jnaauhT"

",".Join(listValue); //like string.Join(",", listValue);

StringEx.IsNumericString("-3.14").Log(); //true
StringEx.IsNumericString("1,000,000.34", ',').Log(); //true

string lorem = StringEx.Lorem; // "lorem ipsum dolor sit"
string lorem = StringEx.LoremShort; // "lorem ipsum dolor sit"
string lorem = StringEx.LoremLong; // "lorem ipsum dolor sit..."
string lorem = StringEx.LoremIpsum(minWords: 4, maxWords: 64, minSentences: 1, maxSentences: 4, numParagraphs: 4); // "lorem ipsum dolor sit..."

//Convert a number to words:
1000.ToWords(); // "one thousand"

//Set Language
Console.OutputEncoding = Encoding.UTF8;

StringEx.SetLanguageToWords(LangWords.VN);

1000.ToWords(); // "một nghìn"

"abc".Contains(x => x.TextOnly).Log(); //true
"123abc".Contains(x => x.TextOnly).Log(); //false
"abc".Contains(x => x.Number).Log(); //false
"123abc".Contains(x => x.NumberOnly).Log(); //false
"123abc".Contains(x => x.Number).Log(); //true

//Simple algorithm for calculating string similarity
var levenshtein = StringAnalysis.LevenshteinDistance("võ thành thuận", "võ thành thuặn");// 1

```

### Number
```csharp
int num = 69;
bool isEven = num.IsEven(); // false

int num2 = 96;
bool isOdd = num2.IsOdd(); // true

"69".ParseInt();
"6.9".ParseFloat();
"3.14".ParseDouble();
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

///Custom function
//One parameter
CalculateEx.AddSimpleFunction("addone", num =>
{
    return ++num;
});
//Many parameter
CalculateEx.AddFunction("sum", agrs =>
{
    return agrs.Sum();
});
//Operator
CalculateEx.AddOperator('?', (a, b) =>
{
    return Random.Shared.Next((int)a, (int)b);
}, 3);

CalculateEx.AddOperator('#', Math.Max, 3);


"addone(1)> ".Log("addone(1)".Calculate()); //2
"1?100> ".Log("1?100".Calculate());
"sum(1;2;3;4;5;6)> ".Log("sum(1;2;3;4;5;6)".Calculate()); //21
"1#2#3#6#5#4> ".Log("1#2#3#6#5#4".Calculate()); //6

//Degree and Radian
CalculateEx.AddSimpleFunction("circle", (num, isDeg) =>
{
    return isDeg ? num*360 : num;
});
CalculateEx.AddFunction("circleSum", (agrs, isDeg) =>
{
    return isDeg ? agrs.Sum() * 360 : agrs.Sum();
});
"circle(1/8deg)".Calculate(); //45
"circleSum(1/8;1/8deg)".Calculate(); //90

//Change CultureInfo
"3.14+1".Calculate().Log(); //4.14
CalculateEx.Culture = new CultureInfo("vi-VN");
"3.14+1".Calculate().Log(); //315
"3,14+1".Calculate().Log(); //4.14
```

### DataTable To List
```csharp
var list = dataTable.ToList<model>();

//Match column name
var list2 = dataTable.ToList<model>(matchCase: true);

//Convert with cache
var list2 = dataTable.ToListCache<model>(matchCase: true);

//Convert List<Dictionary<string, object?>> to DataTable
var dic = new List<Dictionary<string, object?>>
        {
            new() { { "ID", 1 }, { "Name", "John" }, { "Age", 30 } },
            new() { { "ID", 2 }, { "Name", "Alice" }, { "Age", 25 } },
            new() { { "ID", 3 }, { "Name", "Bob" }, { "Age", null } }
        };
var table = dic.ToDataTable();
var valueMap = table.ToList<TestTable>();

//Specify the column name other fields
class TestTable
{
    [MapColumnName("Id")]
    public string Idx { get; set; } = "";
    [IgnoreMapColumnName]
    public string Name { get; set; } = "";
    public string Age { get; set; } = "";
}
```

### String Analysis
The `StringAnalysis` library provides powerful methods for identifying and manipulating patterns within strings, offering enhanced string processing capabilities. Below are examples of some key functions:
#### 1. Function Extraction
The `Functions` method extracts function information, including function name, parameters, and code block from the input string.
```csharp
var function = StringAnalysis.Functions("testfunc(this is param){this is code}");//{FuncName: testfunc, Param: this is param, Code: this is code}
```
#### 2. Function Call Extraction
The `FunctionsCall` method retrieves function calls and their corresponding parameters from the input string.
```csharp
var functionCall = StringAnalysis.FunctionsCall("testfunc1(param one)testfunc2(param two)");//[(testfunc1,param one), (testfunc2,param two)]
```
#### 3. Function Parameters Extraction
The `FunctionParams` method extracts parameters enclosed within parentheses from the input string.
```csharp
var functionParams = StringAnalysis.FunctionParams("(param one)(param two)(param three)");//[param one, param two,param three]
```
#### 4. Language-based Replacement
The ReplaceByLanguage method performs language-based replacements in the input string. In the provided example, if the current display language is set to Vietnamese (vi-VN), the method will replace language codes with their corresponding values, returning "Tiếng Việt" as the result.
```csharp
var replaceByLang = StringAnalysis.ReplaceByLanguage("vi(Tiếng Việt)en(Tiếng anh)");// Example: If the current display language is vi-VN, the result will be "Tiếng Việt"
```
#### 5. Custom Function-based Replacement
The ReplaceByFunc method enables custom replacements based on a specified function. In this example, it replaces occurrences of calc with the result of the provided calculation.
```csharp
var replaceByFunc = StringAnalysis.ReplaceByFunc("the result of the calculation 3*6 is calc(3*6)","calc", data =>
{
    return data.Calculate().ToString();
}); //the result of the calculation 3*6 is 18
```
### TypeEx
TypeEx adds a few small extensions to help make data processing simpler.
```csharp
    var defaultValue = typeof(T).GetDefaultValue(); //T is generic type
```

### DataRow (DataTableEx)
The extensions (Get, Set) for DataRow will help you simplify retrieving data and assigning data to DataRow. You no longer have to handle null and DBNull cases yourself, making the code more compact.
```csharp
    var row = DataTable.Rows[0];
    var val1 = row.GetValue<int>("id");
    var val2 = row.GetStringValue("name");
    row.SetValue("name", "vothuan");
```
Feel free to explore the full range of functionalities offered by the StringAnalysis library to enhance your string processing tasks.

v2 for standard 2.1
v7 for .NET7
# V7
Add the Params class. Similar to a Dictionary, but more convenient.
```csharp
var p = new Params();
var p1 = new Params(("key", "value"), ("key1", "value1"))
var val1 = p1.Get<string>("key"); //value

var p2 = new Params(1,2,3,4,5,6,7,8,9)
var val2 = p2[3]; //4
```
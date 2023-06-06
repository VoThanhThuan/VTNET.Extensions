using VTNET.Extensions;

//CalculateExtension.AddOperator('s', (a, b) =>
//{
//    return Random.Shared.Next((int)a, (int)b);
//});
//"sin(30)+sin(60)".Calculate().Log();

//CalculateExtension.AddSimpleFunction("addone", num =>
//{
//    return ++num;
//});

//CalculateExtension.AddFunction("sum", agrs =>
//{
//    return agrs.Sum();
//});

//CalculateExtension.AddOperator('?', (a, b) =>
//{
//    return Random.Shared.Next((int)a, (int)b);
//}, 3);

//CalculateExtension.AddOperator('#', Math.Max, 3);

//"addone(1)> ".Log("addone(1)".Calculate());
//"1?100> ".Log("1?100".Calculate());
//"sum(a,2,3,4,5,6)> ".Log("sum(a,2,3,4,5,6)".Calculate());
//"1#2#3#6#5#4> ".Log("1#2#3#6#5#4".Calculate());

//"sin(30deg)> ".Log("sin(30deg)".Calculate());
//"tan(30deg)> ".Log("tan(30deg)".Calculate());
//"cos(30deg)> ".Log("cos(30deg)".Calculate());
//"log(30deg)> ".Log("log(30deg)".Calculate());

var isRun = true;
Console.WriteLine("--**Calculate**--");
Console.WriteLine("Support: sin(), tan(), cos(), log(), ^, %, !");
var str = "";
while (isRun)
{
    Console.Write(">> ");
    str = Console.ReadLine();
    "==>> ".Log(str.Calculate());

}
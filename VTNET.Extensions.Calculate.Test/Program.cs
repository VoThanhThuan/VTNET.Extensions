﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;
using VTNET.Extensions;


CalculateEx.AddSimpleFunction("addone", num =>
{
    return ++num;
});
CalculateEx.AddSimpleFunction("circle", (num, isDeg) =>
{
    return isDeg ? num*360 : num;
});

CalculateEx.AddFunction("sum", agrs =>
{
    return agrs.Sum();
});

CalculateEx.AddFunction("circleSum", (agrs, isDeg) =>
{
    return isDeg ? agrs.Sum() * 360 : agrs.Sum();
});

//CalculateEx.AddOperator('#', Math.Max, 3);


var isRun = true;
Console.WriteLine("--**Calculate**--");
Console.WriteLine("Support: sin(), tan(), cos(), log(), ^, %, !");
var str = "";
while (isRun)
{
    Console.Write(">> ");
    str = Console.ReadLine();
    switch (str)
    {
        case "/cul":
            Console.Write("What's culture?(ex: 'vi-VN')>> ");
            str = Console.ReadLine();
            if (!str.IsNullOrEmpty())
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(str);
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(str);
            }
            break;
        case "/cul/cal":
            Console.Write("What's culture?(ex: 'vi-VN')>> ");
            str = Console.ReadLine();
            if (!str.IsNullOrEmpty())
            {
                CalculateEx.Culture = new CultureInfo(str);
            }
            break;
        default:
            if (!str.IsNullOrEmpty())
            {
                "==>> ".Log(str.Calculate());
            }
            break;

    }
}
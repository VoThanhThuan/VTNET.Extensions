namespace VTNET.Extensions.Calculate.Test;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void ManyBrackets()
    {
        var result = "1+(2+(3+(4+(5+(6+(7+(8+(9+1))))))))".Calculate();
        Assert.IsFalse(double.IsNaN(result), "result: NaN");
        Assert.AreEqual(1 + (2 + (3 + (4 + (5 + (6 + (7 + (8 + (9 + 1)))))))), result);
    }
    [TestMethod]
    public void Negative()
    {
        var result = "1-2".Calculate();
        Assert.IsFalse(double.IsNaN(result), "result: NaN");
        Assert.AreEqual(1-2, result);
    }    
    
    [TestMethod]
    public void Decimal()
    {
        var result = "0.1+0.2".Calculate();
        Assert.IsFalse(double.IsNaN(result), "result: NaN");
        Assert.AreEqual(0.3, result);
    }    
    
    [TestMethod]
    public void DecimalNegative()
    {
        Console.WriteLine("case 1");
        var result = "-0.1+-0.2".Calculate();
        Assert.IsFalse(double.IsNaN(result), "result: NaN");
        Assert.AreEqual(-0.3, result);
        
        Console.WriteLine("case 2");
        var result1 = "0.1--0.2".Calculate();
        Assert.IsFalse(double.IsNaN(result1), "result: NaN");
        Assert.AreEqual(0.3, result1);

        Console.WriteLine("case 3");
        var result2 = "-0.1--0.2".Calculate();
        Assert.IsFalse(double.IsNaN(result2), "result: NaN");
        Assert.AreEqual(0.1, result2);

        Console.WriteLine("case 4");
        var result3 = "0.1--+0.2".Calculate();
        Assert.IsFalse(double.IsNaN(result3), "result: NaN");
        Assert.AreEqual(0.3, result3);

        Console.WriteLine("Case 5");
        var result4 = "-1-(-2)".Calculate();
        Assert.IsFalse(double.IsNaN(result4), "result: NaN");
        Assert.AreEqual(-1 - (-2), result4);

    }

    [TestMethod]
    public void AddVariable()
    {
        CalculateEx.AddVariable("không", 0);
        CalculateEx.AddVariable("một", 1);
        CalculateEx.AddVariable("hai", 2);
        CalculateEx.AddVariable("ba", 3);
        CalculateEx.AddVariable("bốn", 4);
        CalculateEx.AddVariable("năm", 5);
        CalculateEx.AddVariable("sáu", 6);
        CalculateEx.AddVariable("bảy", 7);
        CalculateEx.AddVariable("tám", 8);
        CalculateEx.AddVariable("chín", 9);
        var result = "một + hai * (ba + bảy)".Calculate();
        Assert.IsFalse(double.IsNaN(result), "result: NaN");
        Assert.AreEqual(1 + 2 * (3 + 7), result);
    }

    [TestMethod]
    public void CheckParamsRequired()
    {
        CalculateEx.AddFunction("xx", nums =>
        {
            return nums[0] + nums[1];
        }, parameterRequired: 2);

        var result = "xx(1;2;3)".Calculate();
        Assert.IsFalse(double.IsNaN(result), "result: NaN");
        Assert.AreEqual(3, result);

        CalculateEx.AddFunction("xxxx", nums =>
        {
            return nums[0] + nums[1] + nums[2];
        }, parameterRequired: 3);

        var result2 = "xxx(1;2)".Calculate();
        Assert.IsTrue(double.IsNaN(result2), "result: NaN");
    }
    [TestMethod]
    public void TestFunc()
    {
        var result = "sin(60)+sin(60)".CalculateM();
        Assert.AreEqual(result, ((decimal)Math.Sin(60) + (decimal)Math.Sin(60)));

        var result2 = "sin(60)+sin(60)+1".CalculateM();
        Assert.AreEqual(result2, ((decimal)Math.Sin(60) + (decimal)Math.Sin(60) + 1));
    }
}

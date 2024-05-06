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
        Assert.AreEqual("0.3", result.ToString());
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
}

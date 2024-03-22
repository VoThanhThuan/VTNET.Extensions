namespace VTNET.Extensions.Calculate.Test;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void AddVariable()
    {
        CalculateEx.AddVariable("mot", 1);
        CalculateEx.AddVariable("hai", 2);
        CalculateEx.AddVariable("ba", 3);
        CalculateEx.AddVariable("bay", 7);
        var result = "mot + hai * (ba + bay)".Calculate();
        Assert.IsFalse(double.IsNaN(result), "result: NaN");
        Assert.AreEqual((1 + 2 * (3 + 7)), result);
    }
}

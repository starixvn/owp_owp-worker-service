namespace OWP.SumReport.UnitTests.OvertimeReport.Helpers;

using SumReport.OvertimeReport.Helpers;
using Xunit;

public class WorkingShiftHelperUnitTests
{
    [Theory]
    [InlineData(2, 2)]
    [InlineData(2.2, 2)]
    [InlineData(2.4, 2.5)]
    [InlineData(2.7, 3)]
    [InlineData(2.9, 3)]
    public void RoundUpOvertime_ReturnCorrectValue(decimal value, decimal expected)
    {
        var actual = WorkingShiftHelper.RoundUpOvertime(value);

        Assert.Equal(expected, actual);
    }
}
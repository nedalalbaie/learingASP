using learingASP.Calculators;

namespace learingASP.Tests;

public class WorkingDaysCalculatorTests
{
    private static DateOnly D(string date) => DateOnly.ParseExact(date, "yyyy-MM-dd");

    [Theory]
    [InlineData("2026-11-15", "2026-11-19", 5)]   // Sunday to Thursday, same week
    [InlineData("2026-11-15", "2026-11-26", 10)]  // Sunday to Thursday of the next week (the assignment's example)
    [InlineData("2026-11-20", "2026-11-21", 0)]   // Friday and Saturday only (the assignment's example)
    [InlineData("2026-11-19", "2026-11-19", 1)]   // a single working day (Thursday)
    [InlineData("2026-11-20", "2026-11-20", 0)]   // a single Friday
    [InlineData("2026-11-19", "2026-11-22", 2)]   // Thursday to Sunday: Thu + Sun
    [InlineData("2026-12-30", "2027-01-04", 4)]   // across a year boundary: Wed, Thu, Sun, Mon
    public void Count_ReturnsExpectedWorkingDays(string start, string end, int expected)
    {
        var result = WorkingDaysCalculator.Count(D(start), D(end));

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Count_EndBeforeStart_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            WorkingDaysCalculator.Count(D("2026-11-19"), D("2026-11-15")));
    }
}
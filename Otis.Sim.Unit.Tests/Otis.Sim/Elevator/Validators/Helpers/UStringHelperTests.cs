using NStack;
using Otis.Sim.Interface.Validators.Helpers;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Validators.Helpers;

/// <summary>
/// Class UStringHelperTests extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class UStringHelperTests : ElevatorTests
{
    [Test]
    [TestCase(null, null)]
    [TestCase("123", 123)]
    [TestCase("abc", null)]
    [TestCase("-1", -1)]
    public void ToInteger_ReturnExpectedResult(string? input, int? expected)
    {
        var result = input == null 
            ? UStringHelper.ToInteger(input)
            : UStringHelper.ToInteger(ustring.Make(input));

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(null, 1, 10, false)]
    [TestCase("abc", 1, 10, false)]
    [TestCase("0", 1, 10, false)]
    [TestCase("15", 1, 10, false)]
    [TestCase("5", 1, 10, true)]
    [TestCase("-1", -1, 10, true)]
    [TestCase("10", -1, 10, true)]
    [TestCase("-8", -9, -7, true)]
    public void IsInRange_ReturnExpectedResult(string? input, int minValue, int maxValue, bool expected)
    {
        var result = input == null
            ? UStringHelper.IsInRange(input, minValue, maxValue)
            : UStringHelper.IsInRange(ustring.Make(input), minValue, maxValue);

        Assert.That(result, Is.EqualTo(expected));
    }
}

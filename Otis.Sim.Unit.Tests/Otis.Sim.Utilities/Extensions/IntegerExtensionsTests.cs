using Otis.Sim.Utilities.Extensions;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Utilities.Extensions;

/// <summary>
/// Class IntegerExtensionsTests, extends the <see cref="ExtensionsTests" /> class.
/// </summary>
public class IntegerExtensionsTests : ExtensionsTests
{
    /// <summary>
    /// Test the LargerThanByDifference function
    /// </summary>
    /// <param name="maxValue"></param>
    /// <param name="minValue"></param>
    /// <param name="testTrue">Assert true (default) else false</param>
    /// <param name="defaultValue">Optional, else default is applied</param>
    [Test]
    [TestCase(9 , 7)]
    [TestCase(12, 7, true, 4)]
    [TestCase(1, -1)]
    [TestCase(1, -4, true, 4)]
    [TestCase(-1, -2)]
    [TestCase(-1, -8, true, 6)]
    // Negative
    [TestCase(8, 8, false)]
    [TestCase(12, 7, false, 5)]
    [TestCase(-1, -1, false)]
    [TestCase(-1, -8, false, 7)]
    public void LargerThanMinimumDifference(int maxValue, int minValue, bool? testTrue = true, int? defaultValue = null)
    {
        bool result = defaultValue != null 
            ? maxValue.LargerThanByDifference(minValue, (int)defaultValue)
            : maxValue.LargerThanByDifference(minValue);

        Assert.That(result, Is.EqualTo(testTrue));
    }

    /// <summary>
    /// Test the ApplyHigherValue function
    /// </summary>
    /// <param name="value"></param>
    /// <param name="higherValue"></param>
    /// <param name="resultValue">The expected result</param>
    [Test]
    [TestCase(0, -1, 0)]
    [TestCase(-2, -1, -1)]
    [TestCase(21, 9, 21)]
    [TestCase(null, -1, -1)]
    public void ApplyHigherValue(int? value, int higherValue, int resultValue)
    {
        int result = value.ApplyHigherValue(higherValue);
        Assert.That(result, Is.EqualTo(resultValue));
    }

    /// <summary>
    /// Test the ApplyLowerValue function
    /// </summary>
    /// <param name="value"></param>
    /// <param name="lowerValue"></param>
    /// <param name="resultValue">The expected result</param>
    [Test]
    [TestCase(0, -1, -1)]
    [TestCase(-2, -1, -2)]
    [TestCase(21, 9, 9)]
    [TestCase(null, -1, -1)]
    public void ApplyLowerValue(int? value, int lowerValue, int resultValue)
    {
        int result = value.ApplyLowerValue(lowerValue);
        Assert.That(result, Is.EqualTo(resultValue));
    }

    /// <summary>
    /// Test the IsInRange function
    /// </summary>
    /// <param name="value"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <param name="testTrue">Assert true (default) else false</param>
    [Test]
    [TestCase(0, 0, 5)]
    [TestCase(5, 0, 5)]
    [TestCase(2, 0, 5)]
    [TestCase(-3, -6, -1)]
    [TestCase(11, -25, 12)]
    // Negative
    [TestCase(1, -6, -1, false)]
    [TestCase(-26, -25, 12, false)]
    [TestCase(null, -25, 12, false)]
    public void IsInRange(int? value, int minValue, int maxValue, bool? testTrue = true)
    {
        bool result = value.IsInRange(minValue, maxValue);
        Assert.That(result, Is.EqualTo(testTrue));
    }
}

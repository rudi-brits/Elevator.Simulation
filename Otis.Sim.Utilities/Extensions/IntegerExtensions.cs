namespace Otis.Sim.Utilities.Extensions;

/// <summary>
/// IntegerExtensions class
/// </summary>
public static class IntegerExtensions
{
    /// <summary>
    /// LargerThanByDifference
    /// </summary>
    /// <param name="maxValue"></param>
    /// <param name="minValue"></param>
    /// <param name="difference"></param>
    /// <returns>The boolean result</returns>
    public static bool LargerThanByDifference(this int maxValue, int minValue, int difference = 0) =>
       maxValue - minValue > difference;

    /// <summary>
    /// ApplyHigherValue
    /// </summary>
    /// <param name="value"></param>
    /// <param name="higherValue"></param>
    /// <returns>The int result</returns>
    public static int ApplyHigherValue(this int value, int higherValue) =>
        value < higherValue ? higherValue : value;

    /// <summary>
    /// ApplyHigherValue
    /// </summary>
    /// <param name="value"></param>
    /// <param name="higherValue"></param>
    /// <returns>The int result</returns>
    public static int ApplyHigherValue(this int? value, int higherValue)
    {
        if (!value.HasValue)
            return higherValue;

        return value.Value.ApplyHigherValue(higherValue);
    }

    /// <summary>
    /// ApplyLowerValue
    /// </summary>
    /// <param name="value"></param>
    /// <param name="lowerValue"></param>
    /// <returns>The int result</returns>
    public static int ApplyLowerValue(this int value, int lowerValue) =>
      value > lowerValue ? lowerValue : value;

    /// <summary>
    /// ApplyLowerValue
    /// </summary>
    /// <param name="value"></param>
    /// <param name="lowerValue"></param>
    /// <returns>The int result</returns>
    public static int ApplyLowerValue(this int? value, int lowerValue)
    {
        if (!value.HasValue)
            return lowerValue;

        return value.Value.ApplyLowerValue(lowerValue);
    }

    /// <summary>
    /// IsInRange
    /// </summary>
    /// <param name="value"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns>The bool result</returns>
    public static bool IsInRange(this int value, int minValue, int maxValue) =>
        value >= minValue && value <= maxValue;

    /// <summary>
    /// IsInRange
    /// </summary>
    /// <param name="value"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns>The bool result</returns>
    public static bool IsInRange(this int? value, int minValue, int maxValue)
    {
        if (!value.HasValue)
            return false;

        return IsInRange(value.Value, minValue, maxValue);
    }
}

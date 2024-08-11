using NStack;
using Otis.Sim.Utilities.Extensions;

namespace Otis.Sim.Interface.Validators.Helpers;

/// <summary>
/// UStringHelper class
/// </summary>
public static class UStringHelper
{
    /// <summary>
    /// ToInteger
    /// </summary>
    /// <param name="stringValue"></param>
    /// <returns>The int? result</returns>
    public static int? ToInteger(ustring? stringValue)
    {
        if (stringValue == null)
            return null;

        if (int.TryParse(stringValue.ToString(), out int numericValue))
            return numericValue;

        return null;
    }

    /// <summary>
    /// IsInRange
    /// </summary>
    /// <param name="stringValue"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns>The int? result</returns>
    public static bool IsInRange(ustring? stringValue, int minValue, int maxValue)
    {
        var numberValue = ToInteger(stringValue);
        if (!numberValue.HasValue)
            return false;

        return numberValue.Value.IsInRange(minValue, maxValue);
    }
}

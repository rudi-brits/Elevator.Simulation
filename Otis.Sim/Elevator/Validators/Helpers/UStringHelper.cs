using NStack;
using Otis.Sim.Utilities.Extensions;

namespace Otis.Sim.Interface.Validators.Helpers
{
    public static class UStringHelper
    {
        public static int? ToInteger(ustring? stringValue)
        {
            if (stringValue == null)
            {
                return null;
            }

            if (int.TryParse(stringValue.ToString(), out int numericValue))
            {
                return numericValue;
            }
            return null;
        }

        public static bool IsInRange(ustring? stringValue, int minValue, int maxValue)
        {
            var numberValue = ToInteger(stringValue);
            if (!numberValue.HasValue)
            {
                return false;
            }

            return numberValue.Value.IsInRange(minValue, maxValue);
        }
    }
}

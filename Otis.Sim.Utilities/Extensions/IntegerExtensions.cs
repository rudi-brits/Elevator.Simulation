namespace Otis.Sim.Utilities.Extensions
{
    public static class IntegerExtensions
    {
        public static bool LargerThanByDifference(this int maxValue, int minValue, int difference = 0) =>
           maxValue - minValue > difference;

        public static int ApplyHigherValue(this int value, int higherValue) =>
            value < higherValue ? higherValue : value;

        public static int ApplyHigherValue(this int? value, int higherValue)
        {
            if (!value.HasValue)
                return higherValue;

            return value.Value.ApplyHigherValue(higherValue);
        }

        public static int ApplyLowerValue(this int value, int lowerValue) =>
          value > lowerValue ? lowerValue : value;

        public static int ApplyLowerValue(this int? value, int lowerValue)
        {
            if (!value.HasValue)
                return lowerValue;

            return value.Value.ApplyLowerValue(lowerValue);
        }

        public static bool IsInRange(int value, int minValue, int maxValue) =>
            value >= minValue && value <= maxValue;

        public static bool IsInRange(int? value, int minValue, int maxValue)
        {
            if (!value.HasValue)
                return false;

            return IsInRange(value.Value, minValue, maxValue);
        }
    }
}

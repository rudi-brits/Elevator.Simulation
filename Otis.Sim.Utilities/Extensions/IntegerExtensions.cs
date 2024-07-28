namespace Otis.Sim.Utilities.Extensions
{
    public static class IntegerExtensions
    {
        public static bool LargerThanMinimumDifference(this int maxValue, int minValue, int minimumDifference = 0) =>
           maxValue - minValue > minimumDifference;

        public static int ApplyHigherValue(this int value, int higherValue) =>
            value < higherValue ? higherValue : value;

        public static int ApplyHigherValue(this int? value, int higherValue)
        {
            if (!value.HasValue)
                return higherValue;

            return value.ApplyHigherValue(higherValue);
        }

        public static int ApplyLowerValue(this int value, int lowerValue) =>
          value > lowerValue ? lowerValue : value;

        public static int ApplyLowerValue(this int? value, int lowerValue)
        {
            if (!value.HasValue)
                return lowerValue;

            return value.ApplyLowerValue(lowerValue);
        }

        public static bool IsInRange(int value, int minValue, int maxValue) =>
            value >= minValue && value <= maxValue;

        public static bool IsInRange(int? value, int minValue, int maxValue)
        {
            if (!value.HasValue)
                return false;

            return IsInRange(value, minValue, maxValue);
        }
    }
}

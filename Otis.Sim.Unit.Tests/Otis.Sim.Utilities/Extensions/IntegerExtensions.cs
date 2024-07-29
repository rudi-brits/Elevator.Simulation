using Otis.Sim.Utilities.Extensions;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Utilities.Extensions
{
    [TestFixture]
    public class IntegerExtensionsTests
    {
        const string IntegerExtensions_LargerThanMinimumDifference = nameof(IntegerExtensions_LargerThanMinimumDifference);
        const string IntegerExtensions_ApplyHigherValue = nameof(IntegerExtensions_ApplyHigherValue);

        // Positive tests
        [Test]
        [Category(IntegerExtensions_LargerThanMinimumDifference)]
        public void LargerThanMinimumDifference_DefaultMinimumDifference_True()
        {
            int maxValue = 9;
            int minValue = 7;

            bool result = maxValue.LargerThanByDifference(minValue);
            Assert.IsTrue(result);
        }

        [Test]
        [Category(IntegerExtensions_LargerThanMinimumDifference)]
        public void LargerThanMinimumDifference_CustomMinimumDifference_True()
        {
            int maxValue = 12;
            int minValue = 7;

            bool result = maxValue.LargerThanByDifference(minValue, maxValue - minValue - 1);
            Assert.IsTrue(result);
        }

        [Test]
        [Category(IntegerExtensions_LargerThanMinimumDifference)]
        public void LargerThanMinimumDifference_NegativeValue_True()
        {
            int maxValue = 1;
            int minValue = -1;

            bool result = maxValue.LargerThanByDifference(minValue);
            Assert.IsTrue(result);
        }

        [Test]
        [Category(IntegerExtensions_LargerThanMinimumDifference)]
        public void LargerThanMinimumDifference_NegativeValues_True()
        {
            int maxValue = -1;
            int minValue = -2;

            bool result = maxValue.LargerThanByDifference(minValue);
            Assert.IsTrue(result);
        }

        [Test]
        [Category(IntegerExtensions_LargerThanMinimumDifference)]
        public void LargerThanMinimumDifference_CustomMinimumDifference_NegativeValues_True()
        {
            int maxValue = -1;
            int minValue = -8;

            bool result = maxValue.LargerThanByDifference(minValue, maxValue - minValue - 1);
            Assert.IsTrue(result);
        }

        [Test]
        [Category(IntegerExtensions_ApplyHigherValue)]
        public void ApplyHigherValue_UseNegative_True()
        {
            int value = 0;
            int higherValue = -1;

            int result = value.ApplyHigherValue(higherValue);
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        [Category(IntegerExtensions_ApplyHigherValue)]
        public void ApplyHigherValue_UseNegatives_True()
        {
            int value = -2;
            int higherValue = -1;

            int result = value.ApplyHigherValue(higherValue);
            Assert.That(result, Is.EqualTo(higherValue));
        }

        [Test]
        [Category(IntegerExtensions_ApplyHigherValue)]
        public void ApplyHigherValue_UsePositives_True()
        {
            int value = 21;
            int higherValue = 9;

            int result = value.ApplyHigherValue(higherValue);
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        [Category(IntegerExtensions_ApplyHigherValue)]
        public void ApplyHigherValueNullable_UseNegative_True()
        {
            int? value = null;
            int higherValue = -1;

            int result = value.ApplyHigherValue(higherValue);
            Assert.That(result, Is.EqualTo(higherValue));
        }

        // Negative tests
        [Test]
        [Category(IntegerExtensions_LargerThanMinimumDifference)]
        public void LargerThanMinimumDifference_DefaultMinimumDifference_False()
        {
            int maxValue = 8;
            int minValue = 8;

            bool result = maxValue.LargerThanByDifference(minValue);
            Assert.IsFalse(result);
        }

        [Test]
        [Category(IntegerExtensions_LargerThanMinimumDifference)]
        public void LargerThanMinimumDifference_CustomMinimumDifference_False()
        {
            int maxValue = 12;
            int minValue = 7;

            bool result = maxValue.LargerThanByDifference(minValue, maxValue - minValue);
            Assert.IsFalse(result);
        }

        [Test]
        [Category(IntegerExtensions_LargerThanMinimumDifference)]
        public void LargerThanMinimumDifferenc_NegativeValue_False()
        {
            int maxValue = -1;
            int minValue = -1;

            bool result = maxValue.LargerThanByDifference(minValue);
            Assert.IsFalse(result);
        }

        [Test]
        [Category(IntegerExtensions_LargerThanMinimumDifference)]
        public void LargerThanMinimumDifference_CustomMinimumDifference_NegativeValues_False()
        {
            int maxValue = -1;
            int minValue = -8;

            bool result = maxValue.LargerThanByDifference(minValue, maxValue - minValue);
            Assert.IsFalse(result);
        }
    }
}

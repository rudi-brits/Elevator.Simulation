using Otis.Sim.Utilities.Helpers;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Utilities.Helpers;

/// <summary>
/// Class ReflectionHelperTests extends the <see cref="HelperTests" /> class.
/// </summary>
public class ReflectionHelperTests : HelperTests
{
    /// <summary>
    /// A sample class for testing.
    /// </summary>
    private class SampleTestClass
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhysicalStreetAddress { get; set; }
        public int Age { get; set; }
    }

    /// <summary>
    /// The number of properties in the sample class
    /// </summary>
    private int _numberOfProperties;

    /// <summary>
    /// Class constructor
    /// </summary>
    public ReflectionHelperTests()
    {
        _numberOfProperties = GetProperties<SampleTestClass>()
            .Length;
    }

    /// <summary>
    /// Test that the list of formatted property names are not empty.
    /// </summary>
    [Test]
    public void FormattedPropertyNames_NotEmpty_CorrectCount()
    {
        var propertyNames = ReflectionHelper.GetFormattedPropertyNames<SampleTestClass>();

        Assert.That(propertyNames, Is.Not.Null.And.Count.EqualTo(_numberOfProperties),
            $"List of property names must have a length of {_numberOfProperties}");
    }
}

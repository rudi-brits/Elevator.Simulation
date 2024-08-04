using Otis.Sim.Unit.Tests.Otis.Sim.Utilities.Extensions;
using Otis.Sim.Utilities.Helpers;
using System.Reflection;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Utilities.Helpers;

/// <summary>
/// Class ReflectionHelperTests, extends the <see cref="ExtensionsTests" /> class.
/// </summary>
public class ReflectionHelperTests : ExtensionsTests
{
    /// <summary>
    /// A sample class for testing
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
    /// A dictionary of properties and camel formatted names from the sample class
    /// </summary>
    private Dictionary<string, string> _propertyNameDictionary;

    /// <summary>
    /// Class constructor
    /// </summary>
    public ReflectionHelperTests()
    {
        _numberOfProperties = typeof(SampleTestClass)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Length;

        _propertyNameDictionary = new Dictionary<string, string>()
        {
            { nameof(SampleTestClass.FirstName), "First Name" },
            { nameof(SampleTestClass.LastName), "Last Name" },
            { nameof(SampleTestClass.PhysicalStreetAddress), "Physical Street Address" },
            { nameof(SampleTestClass.Age), "Age" },
        };
    }

    /// <summary>
    /// Test that the list of formatted property names are not empty
    /// </summary>
    [Test]
    public void GetFormattedPropertyNames_Not_Empty()
    {
        var propertyNames = ReflectionHelper.GetFormattedPropertyNames<SampleTestClass>();

        Assert.That(propertyNames, Is.Not.Null.And.Count.EqualTo(_numberOfProperties),
            $"Formatted property names must have a length of {_numberOfProperties}");

        foreach(string propertyName in propertyNames)
        {
            if (!_propertyNameDictionary.ContainsValue(propertyName))
                Assert.Fail($"The formatted value {propertyName} does not match any expected values");
        }
    }

    /// <summary>
    /// Test that the list of formatted property names all match the expected values
    /// </summary>
    [Test]
    public void GetFormattedPropertyNames_Match()
    {
        var propertyNames = ReflectionHelper.GetFormattedPropertyNames<SampleTestClass>();

        foreach (string propertyName in propertyNames)
        {
            if (!_propertyNameDictionary.ContainsValue(propertyName))
                Assert.Fail($"The formatted value {propertyName} does not match any expected values");
        }
    }
}

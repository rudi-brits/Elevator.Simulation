using Otis.Sim.Configuration.Models;
using System.Reflection;

namespace Otis.Sim.Unit.Tests.Configuration.Models;

/// <summary>
/// Class BuildingConfigurationTests, extends the <see cref="ConfigurationTests" /> class.
/// </summary>
public class BuildingConfigurationTests : ConfigurationTests
{
    /// <summary>
    /// Test instance of the <see cref="BuildingConfiguration" /> class.
    /// </summary>
    private BuildingConfiguration _buildingConfiguration;

    /// <summary>
    /// Class constructor initialising and instance of the <see cref="BuildingConfiguration" /> class.
    /// </summary>
    public BuildingConfigurationTests()
    {
        _buildingConfiguration = new BuildingConfiguration()
        {
            LowestFloor = 10,
            HighestFloor = 20,
            MaximumElevatorLoad = 30
        };
    }

    /// <summary>
    /// Test that ToString is not empty.
    /// </summary>
    [Test]
    [Order(1)]
    public void ToString_NotNullOrEmpty()
    {
        var result = _buildingConfiguration.ToString();
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }

    /// <summary>
    /// Test that ToString contains all instance properties and values.
    /// </summary>
    [Test]
    [Order(2)]
    public void ToString_HasAllPropertiesAndValues()
    {
        var result = _buildingConfiguration.ToString();

        var properties = typeof(BuildingConfiguration)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        Assert.That(properties, Is.Not.Null.And.Not.Empty, "The list of properties is empty");

        foreach (var property in properties)
        {
            var propertyName = property.Name;
            if (!result.Contains(propertyName))
                Assert.Fail($"{result} does not contain the property {propertyName}");

            var propertyInfo = typeof(BuildingConfiguration).GetProperty(propertyName);
            var propertyValue = propertyInfo?.GetValue(_buildingConfiguration);

            Assert.That(propertyValue, Is.Not.Null, 
                $"The value of {propertyName} in the model is null");

            if (!result.Contains($"{propertyValue}"))
                Assert.Fail($"The value of {propertyName} was not found in {result}");
        }
    }
}

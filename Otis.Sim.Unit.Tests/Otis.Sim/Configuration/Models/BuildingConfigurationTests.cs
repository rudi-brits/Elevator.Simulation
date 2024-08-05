using Otis.Sim.Configuration.Models;

namespace Otis.Sim.Unit.Tests.Configuration.Models;

/// <summary>
/// Class BuildingConfigurationTests extends the <see cref="ConfigurationTests" /> class.
/// </summary>
public class BuildingConfigurationTests : ConfigurationTests
{
    /// <summary>
    /// A test instance of the <see cref="BuildingConfiguration" /> class.
    /// </summary>
    private BuildingConfiguration _buildingConfiguration = new BuildingConfiguration()
    {
        LowestFloor         = 10,
        HighestFloor        = 20,
        MaximumElevatorLoad = 30
    };

    /// <summary>
    /// Ensure properties can be initialised as zero.
    /// </summary>
    [Test]
    public void DefaultInitialisation_ShouldBeZero()
    {
        var result = new BuildingConfiguration();
        Assert.That(result.LowestFloor, Is.EqualTo(0));
        Assert.That(result.HighestFloor, Is.EqualTo(0));
        Assert.That(result.MaximumElevatorLoad, Is.EqualTo(0));
    }

    /// <summary>
    /// Ensure properties' values are assigned.
    /// </summary>
    [Test]
    public void PropertyAssignments_ShouldHaveAssignedValues()
    {
        Assert.That(_buildingConfiguration.LowestFloor, Is.EqualTo(10));
        Assert.That(_buildingConfiguration.HighestFloor, Is.EqualTo(20));
        Assert.That(_buildingConfiguration.MaximumElevatorLoad, Is.EqualTo(30));
    }

    /// <summary>
    /// Test that ToString is not empty.
    /// </summary>
    [Test]
    public void ToString_NotNullOrEmpty()
    {
        var result = _buildingConfiguration.ToString();
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }

    /// <summary>
    /// Test that ToString contains all instance properties and values.
    /// </summary>
    [Test]
    public void ToString_HasAllPropertiesAndValues()
    {
        var result = _buildingConfiguration.ToString();

        var properties = GetProperties<BuildingConfiguration>();
        Assert.That(properties, Is.Not.Null.And.Not.Empty, "The list of properties is empty");

        foreach (var property in properties)
        {
            var propertyName = property.Name;
            if (!result.Contains(propertyName))
                Assert.Fail($"{result} does not contain the property {propertyName}");

            var propertyInfo = typeof(BuildingConfiguration).GetProperty(propertyName);

            var propertyValue = propertyInfo?.GetValue(_buildingConfiguration);
            Assert.That(propertyValue, Is.Not.Null, 
                $"The value of {propertyName} in the is null in {result}");

            if (!result.Contains($"{propertyValue}"))
                Assert.Fail($"The value of {propertyName} was not found in {result}");
        }
    }
}

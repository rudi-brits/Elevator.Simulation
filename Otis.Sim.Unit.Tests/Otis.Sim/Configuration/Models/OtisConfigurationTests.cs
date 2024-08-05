using Otis.Sim.Configuration.Models;

namespace Otis.Sim.Unit.Tests.Configuration.Models;

/// <summary>
/// Class OtisConfigurationTests, extends the <see cref="ConfigurationTests" /> class.
/// </summary>
public class OtisConfigurationTests : ConfigurationTests
{
    /// <summary>
    /// Ensure properties van be initialised as null.
    /// </summary>
    [Test]
    public void DefaultInitialisation_ShouldBeNull()
    {
        var result = new OtisConfiguration();
        Assert.That(result.BuildingConfiguration, Is.Null);
        Assert.That(result.ElevatorsConfiguration, Is.Null);
    }

    /// <summary>
    /// Ensure properties' values are assigned.
    /// </summary>
    [Test]
    public void PropertyAssignments_ShouldHaveAssignedValues()
    {
        var buildingConfiguration = new BuildingConfiguration
        {
            LowestFloor         = -2,
            HighestFloor        = 10,
            MaximumElevatorLoad = 15
        };

        var elevatorsConfiguration = new List<ElevatorConfiguration>
        {
            new ElevatorConfiguration { 
                Description  = "Elevator 1",
                LowestFloor  = -2,
                HighestFloor = 10,
            },
            new ElevatorConfiguration {
                Description = "Elevator 2",
                LowestFloor = 2,
            },
            new ElevatorConfiguration {
                Description = "Elevator 3",
                LowestFloor = 8,
            },
        };

        var configuration = new OtisConfiguration
        {
            BuildingConfiguration  = buildingConfiguration,
            ElevatorsConfiguration = elevatorsConfiguration
        };

        // Assert
        Assert.That(configuration.BuildingConfiguration, Is.Not.Null);
        Assert.That(configuration.BuildingConfiguration!.LowestFloor, Is.EqualTo(-2));
        Assert.That(configuration.BuildingConfiguration!.HighestFloor, Is.EqualTo(10));
        Assert.That(configuration.BuildingConfiguration!.MaximumElevatorLoad, Is.EqualTo(15));

        Assert.That(configuration.ElevatorsConfiguration, Is.Not.Null);

        var elevator1 = elevatorsConfiguration[0];
        Assert.That(elevator1.Description, Is.EqualTo("Elevator 1"));
        Assert.That(elevator1.LowestFloor, Is.EqualTo(-2));
        Assert.That(elevator1.HighestFloor, Is.EqualTo(10));

        // Assertions for Elevator 2
        var elevator2 = elevatorsConfiguration[1];
        Assert.That(elevator2.Description, Is.EqualTo("Elevator 2"));
        Assert.That(elevator2.LowestFloor, Is.EqualTo(2));
        Assert.That(elevator2.HighestFloor, Is.Null);

        // Assertions for Elevator 3
        var elevator3 = elevatorsConfiguration[2];
        Assert.That(elevator3.Description, Is.EqualTo("Elevator 3"));
        Assert.That(elevator3.LowestFloor, Is.EqualTo(8));
        Assert.That(elevator3.HighestFloor, Is.Null);
    }
}

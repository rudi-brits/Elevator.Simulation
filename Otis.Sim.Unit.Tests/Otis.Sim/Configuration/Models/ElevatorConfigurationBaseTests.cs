using Otis.Sim.Configuration.Models;

namespace Otis.Sim.Unit.Tests.Configuration.Models;

/// <summary>
/// Class ElevatorConfigurationBaseTests extends the <see cref="ConfigurationTests" /> class.
/// </summary>
public class ElevatorConfigurationBaseTests : ConfigurationTests
{
    /// <summary>
    /// A sample class to test inheritance from the <see cref="ElevatorConfigurationBase" /> class.
    /// </summary>
    private class SampleClass : ElevatorConfigurationBase
    {
    }    

    /// <summary>
    /// Ensure properties van be initialised as null.
    /// </summary>
    [Test]
    public void DefaultInitialisation_ShouldBeNull_OeEmptyString()
    {
        var result = new SampleClass();

        Assert.That(result.Description, Is.EqualTo(string.Empty));
        Assert.That(result.LowestFloor, Is.Null);
        Assert.That(result.HighestFloor, Is.Null);
    }

    /// <summary>
    /// Ensure properties' values are assigned.
    /// </summary>
    [Test]
    public void PropertyAssignments_ShouldHaveAssignedValues()
    {
        var sampleInstance = new SampleClass
        {
            Description  = "Sample test",
            LowestFloor  = 10,
            HighestFloor = 201
        };

        Assert.That(sampleInstance.Description, Is.EqualTo("Sample test"));
        Assert.That(sampleInstance.LowestFloor, Is.EqualTo(10));
        Assert.That(sampleInstance.HighestFloor, Is.EqualTo(201));
    }
}

using Otis.Sim.Elevator.Models;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Models;

/// <summary>
/// Class ElevatorRequestResultTests extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class ElevatorRequestValidationValuesTests : ElevatorTests
{
    /// <summary>
    /// Ensure properties can be initialised correctly.
    /// </summary>
    [Test]
    public void DefaultInitialisation()
    {
        var result = new ElevatorRequestValidationValues();
        Assert.That(result.LowestFloor, Is.EqualTo(0));
        Assert.That(result.HighestFloor, Is.EqualTo(0));
        Assert.That(result.MaximumLoad, Is.EqualTo(0));
    }

    /// <summary>
    /// Ensure properties' values are assigned.
    /// </summary>
    [Test]
    public void PropertyAssignments_ShouldHaveAssignedValues()
    {
        var result = new ElevatorRequestValidationValues()
        {
            LowestFloor  = 2,
            HighestFloor = 6,
            MaximumLoad  = 8
        };

        Assert.That(result.LowestFloor, Is.EqualTo(2));
        Assert.That(result.HighestFloor, Is.EqualTo(6));
        Assert.That(result.MaximumLoad, Is.EqualTo(8));
    }
}

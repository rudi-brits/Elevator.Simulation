using Otis.Sim.Elevator.Models;
using static Otis.Sim.Elevator.Enums.ElevatorEnum;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Models;

/// <summary>
/// Class ElevatorDataRowTests extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class ElevatorDataRowTests : ElevatorTests
{
    /// <summary>
    /// Ensure properties can be initialised correctly.
    /// </summary>
    [Test]
    public void DefaultInitialisation()
    {
        var result = new ElevatorDataRow();
        Assert.That(result.Id, Is.EqualTo(0));
        Assert.That(result.Name, Is.EqualTo(string.Empty));
        Assert.That(result.CurrentFloor, Is.EqualTo(0));
        Assert.That(result.NextFloor, Is.EqualTo(0));
        Assert.That(result.CurrentLoad, Is.EqualTo(0));
        Assert.That(result.Capacity, Is.EqualTo(0));
        Assert.That(result.Status, Is.EqualTo(ElevatorStatus.Idle));
    }

    /// <summary>
    /// Ensure properties' values are assigned.
    /// </summary>
    [Test]
    public void PropertyAssignments_ShouldHaveAssignedValues()
    {
        var result = new ElevatorDataRow()
        {
            Id           = 10,
            Name         = "Elevator 22",
            CurrentFloor = 5,
            NextFloor    = 6,
            CurrentLoad  = 7,
            Capacity     = 3,
            Status       = ElevatorStatus.MovingUp
        };

        Assert.That(result.Id, Is.EqualTo(10));
        Assert.That(result.Name, Is.EqualTo("Elevator 22"));
        Assert.That(result.CurrentFloor, Is.EqualTo(5));
        Assert.That(result.NextFloor, Is.EqualTo(6));
        Assert.That(result.CurrentLoad, Is.EqualTo(7));
        Assert.That(result.Capacity, Is.EqualTo(3));
        Assert.That(result.Status, Is.EqualTo(ElevatorStatus.MovingUp));
    }
}

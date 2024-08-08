using Otis.Sim.Elevator.Models;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Models;

/// <summary>
/// Class UserInputRequestTests extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class UserInputRequestTests : ElevatorTests
{
    /// <summary>
    /// Ensure properties can be initialised correctly.
    /// </summary>
    [Test]
    public void DefaultInitialisation()
    {
        var result = new UserInputRequest();
        Assert.That(result.OriginFloorInput, Is.EqualTo(null));
        Assert.That(result.DestinationFloorInput, Is.EqualTo(null));
        Assert.That(result.CapacityInput, Is.EqualTo(null));
    }

    /// <summary>
    /// Ensure properties' values are assigned.
    /// </summary>
    [Test]
    public void PropertyAssignments_ShouldHaveAssignedValues()
    {
        var result = new UserInputRequest()
        {
            OriginFloorInput      = "10",
            DestinationFloorInput = "-1",
            CapacityInput         = "3"
        };

        Assert.That(result.OriginFloorInput.ToString(), Is.EqualTo("10"));
        Assert.That(result.DestinationFloorInput.ToString(), Is.EqualTo("-1"));
        Assert.That(result.CapacityInput.ToString(), Is.EqualTo("3"));
    }
}

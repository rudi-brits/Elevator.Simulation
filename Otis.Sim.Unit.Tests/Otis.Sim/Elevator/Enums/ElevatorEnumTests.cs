using Otis.Sim.Elevator.Enums;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Enums;

/// <summary>
/// Class ElevatorEnum extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class ElevatorEnumTests : ElevatorTests
{
    /// <summary>
    /// RequestStatus has values.
    /// </summary>
    [Test]
    public void RequestStatus_HasValues()
    {
        var result = Enum.GetValues(typeof(ElevatorEnum.RequestStatus));
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }

    /// <summary>
    /// ElevatorStatus has values.
    /// </summary>
    [Test]
    public void ElevatorStatus_HasValues()
    {
        var result = Enum.GetValues(typeof(ElevatorEnum.ElevatorStatus));
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }

    /// <summary>
    /// ElevatorDirection has values.
    /// </summary>
    [Test]
    public void ElevatorDirection_HasValues()
    {
        var result = Enum.GetValues(typeof(ElevatorEnum.ElevatorDirection));
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }
}

using FluentValidation.Results;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Unit.Tests.Constants;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Models;

/// <summary>
/// Class ElevatorRequestResultTests extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class ElevatorRequestResultTests : ElevatorTests
{
    /// <summary>
    /// Ensure exception on empty constructor.
    /// </summary>
    [Test]
    public void EmptyConstructor_Exception()
    {
        Assert.Throws<MissingMethodException>(() =>
            Activator.CreateInstance(typeof(ElevatorRequestResult), true));
    }

    /// <summary>
    /// Constructor with success parameter.
    /// </summary>
    /// <param name="success"></param>
    [Test]
    [TestCase(null)]
    [TestCase(true)]
    [TestCase(false)]
    public void SuccessParameterConstructor_Valid(bool? success)
    {
        var elevatorRequestResult = success == null
            ? new ElevatorRequestResult()
            : new ElevatorRequestResult((bool)success);

        if (success != false)
            Assert.That(elevatorRequestResult.Success, Is.EqualTo(true));
        else
            Assert.That(elevatorRequestResult.Success, Is.EqualTo(false));
    }

    /// <summary>
    /// Constructor with success and message parameter.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    [Test]
    [TestCase(true, null)]
    [TestCase(false, null)]
    [TestCase(true, "True")]
    [TestCase(false, "False")]
    public void Success_MessageParameterConstructor_Valid(bool success, string message)
    {
        var elevatorRequestResult = new ElevatorRequestResult(success, message);

        Assert.That(elevatorRequestResult.Success, Is.EqualTo(success));
        Assert.That(elevatorRequestResult.Message, Is.EqualTo(message));
    }

    /// <summary>
    /// Constructor with a <see cref="List" /> of <see cref="ValidationFailure" />.
    /// </summary>
    /// <param name="validationErrors"></param>
    [Test]
    public void Success_MessageParameterConstructor_Valid()
    {
        var numberOfFailures = 5;
        var validationErrors = GetMockValidationFailures(numberOfFailures);
        var elevatorRequestResult = new ElevatorRequestResult(validationErrors);

        Assert.That(elevatorRequestResult.Message, Is.Not.Null.Or.Empty);

        var numberOfErrorMessages = SplitByNewLineCharacterLength(elevatorRequestResult.Message);
        if (numberOfFailures != numberOfErrorMessages)
            Assert.Fail(TestConstants.NewLineMessageLengthError(numberOfFailures, numberOfErrorMessages));
    }
}

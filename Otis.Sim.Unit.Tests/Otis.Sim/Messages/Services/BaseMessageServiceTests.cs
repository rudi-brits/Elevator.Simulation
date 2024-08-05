using Otis.Sim.Messages.Services;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Messages.Services;

/// <summary>
/// Class BaseMessageServiceTests extends the <see cref="MessagesTests" /> class.
/// </summary>
/// <category><see cref="MessagesTests" /></category>
public class BaseMessageServiceTests : MessagesTests
{
    /// <summary>
    /// Tests formatting of strings having no interpolations or 
    /// validationg the correct number of parameters.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inputs"></param>
    [Test]
    [TestCase("This has no args", null)]
    [TestCase(ValidationMessageService.DuplicateElevatorRequest, "One", "Two", "Three")]
    public void FormatMessage_NoArgs_OrCorrectNumberOfArgs(string message, params string[] inputs)
    {
        var result = BaseMessageService.FormatMessage(message, inputs);
        Assert.That(result, Is.Not.Null.And.Not.Empty);

        if (_stringInterpolationArgsRegex.IsMatch(result))
            Assert.Fail($"The result {result} contains unresolved interpolations");
    }

    /// <summary>
    /// Tests throwing of a FormatException when providing the incorrect number of 
    /// interpolation parameters.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inputs"></param>
    [Test]
    [TestCase(ValidationMessageService.DuplicateElevatorRequest, "One", "Two")]
    public void FormatMessage_IncorrectNumberOfArgs(string message, params string[] inputs)
    {
        Assert.Throws<FormatException>(() => BaseMessageService.FormatMessage(message, inputs));
    }
}

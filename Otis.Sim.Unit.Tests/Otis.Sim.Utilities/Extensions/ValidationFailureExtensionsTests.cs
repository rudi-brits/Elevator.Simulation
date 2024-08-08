using FluentValidation.Results;
using Otis.Sim.Unit.Tests.Constants;
using Otis.Sim.Utilities.Constants;
using Otis.Sim.Utilities.Extensions;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Utilities.Extensions;

/// <summary>
/// Class ValidationFailureExtensionsTests extends the <see cref="ExtensionsTests" /> class.
/// </summary>
public class ValidationFailureExtensionsTests : ExtensionsTests
{
    /// <summary>
    /// The list of failures
    /// </summary>
    private List<ValidationFailure> _failures;
    /// <summary> 
    /// The number of failures to be generated for testing
    /// </summary>
    private int _failuresCount = 3;

    /// <summary>
    /// Setup before any tests are run
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _failures = GetMockValidationFailures(_failuresCount);
    }

    /// <summary>
    /// Test using an empty failure list
    /// </summary>
    [Test]
    public void ToNewLineString_Empty()
    {
        var result = new List<ValidationFailure>().ToNewLineString();
        Assert.That(result, Is.EqualTo(string.Empty));
    }

    /// <summary>
    /// Test using a failure list with one message
    /// </summary>
    [Test]
    public void ToNewLineString_SingleMessage()
    {
        var result = _failures.Take(1).ToList().ToNewLineString();
        if (result.Contains(UtilityConstants.NewLineCharacter))
            Assert.Fail($"A single message may not contain {UtilityConstants.NewLineCharacter}");
    }

    /// <summary>
    /// Test using a failure list with multiple messages
    /// </summary>
    [Test]
    public void ToNewLineString_MultipleMessages()
    {
        var result         = _failures.Take(_failuresCount).ToList().ToNewLineString();
        var numberOfErrors = SplitByNewLineCharacterLength(result);

        if (numberOfErrors != _failuresCount)
            Assert.Fail(TestConstants.NewLineMessageLengthError(_failuresCount, numberOfErrors));
    }
}

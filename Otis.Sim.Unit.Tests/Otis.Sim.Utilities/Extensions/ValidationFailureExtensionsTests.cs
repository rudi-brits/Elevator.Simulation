using FluentValidation.Results;
using Otis.Sim.Utilities.Constants;
using Otis.Sim.Utilities.Extensions;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Utilities.Extensions;

/// <summary>
/// Class ValidationFailureExtensionsTests, extends the <see cref="ExtensionsTests" /> class.
/// </summary>
public class ValidationFailureExtensionsTests : ExtensionsTests
{
    /// <summary>
    /// A string constant for message
    /// </summary>
    private const string message = nameof(message);
    /// <summary>
    /// A string constant for property
    /// </summary>
    private const string property = nameof(property);

    /// <summary>
    /// The list of failures
    /// </summary>
    private List<ValidationFailure> _failures;
    /// <summary> 
    /// The number of failures to be generated for testing
    /// </summary>
    private int _failuresCount = 3;

    /// <summary>
    /// Class constructor. Generates number of failures using the <see cref="_failuresCount" /> value
    /// </summary>
    public ValidationFailureExtensionsTests()
    {
        _failures = new List<ValidationFailure>();
        for (var i = 0; i < _failuresCount; i++)
            _failures.Add(new ValidationFailure($"{property}{i}", $"{message}{i}"));
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
        var result = _failures.Take(_failuresCount).ToList().ToNewLineString();
        var messageParts = result.Split(new string[] { UtilityConstants.NewLineCharacter }, StringSplitOptions.None);
        if (messageParts.Length != _failuresCount)
            Assert.Fail($"Expected {_failuresCount} new line messages, received {messageParts.Length}");
    }
}

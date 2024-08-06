using Otis.Sim.Messages.Services;
using Otis.Sim.Unit.Tests.Constants;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Messages.Services;

/// <summary>
/// Class ValidationMessageServiceTests extends the <see cref="MessagesTests" /> class.
/// </summary>
public class ValidationMessageServiceTests : MessagesTests
{
    /// <summary>
    /// Regex to test for numbers with an open brace, missing closing brace
    /// </summary>
    private readonly Regex _openBraceWithoutCloseRegex = new Regex(@"\{\d+[^}]*\{", RegexOptions.Compiled);
    /// <summary>
    /// Regex to test for numbers with a closing brace, missing open brace
    /// </summary>
    private readonly Regex _closeBraceWithoutOpenRegex = new Regex(@"^[^{]*\d+\}", RegexOptions.Compiled);
    /// <summary>
    /// Indicates whether the previous ordered test succeeded
    /// </summary>
    private bool _previousTestSucceeded = true;
    /// <summary>
    /// A list of all constant string properties of the <see cref="ValidationMessageService" /> class
    /// </summary>
    IEnumerable<FieldInfo> _fieldInfo;

    /// <summary>
    /// Setup before any tests are run.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _fieldInfo = GetPublicStaticLiteralFieldsInfo<ValidationMessageService>();
    }

    /// <summary>
    /// Get all properties from <see cref="ValidationMessageService" />
    /// </summary>
    [Test]
    [Order(0)]
    public void GetFieldsInfo_True()
    {
        if (_fieldInfo?.Any() != true)
        {
            _previousTestSucceeded = false;
            Assert.Fail(TestConstants.EmptyLiteralFieldsMessage);
            return;
        }
    }        

    [Test]
    /// <summary>
    /// Validate the format using <see cref="_openBraceWithoutCloseRegex" />, <see cref="_closeBraceWithoutOpenRegex" />,
    /// <see cref="_validPatternRegex" />, ensuring that indexes are unique, and in range
    /// </summary>
    [Order(1)]
    public void FieldsFormat_Valid()
    {
        if (!_previousTestSucceeded)
        {
            HandlePreviousTestFailed(nameof(FieldsFormat_Valid));
            return;
        }

        foreach (var validationConstant in _fieldInfo)
        {
            var constantValue = $"{validationConstant.GetRawConstantValue()}";
            if (string.IsNullOrWhiteSpace(constantValue))
            {
                Assert.Fail($"{validationConstant.Name} {TestConstants.HasAnEmptyValue}");
                return;
            }

            if (ValidateFormat(validationConstant.Name, constantValue))
                ValidateIndexes(validationConstant.Name, constantValue);
        }
    }

    /// <summary>
    /// Validate the format as defined in <see cref="_openBraceWithoutCloseRegex" /> and 
    /// <see cref="_closeBraceWithoutOpenRegex" />
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private bool ValidateFormat(string fieldName, string value)
    {
        if (_openBraceWithoutCloseRegex.IsMatch(value))
        {
            AssertFailInterpolationCurlyBrace(fieldName, "closing");
            return false;
        }
        if (_closeBraceWithoutOpenRegex.IsMatch(value))
        {
            AssertFailInterpolationCurlyBrace(fieldName, "opening");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validate the uniqueness and range of the indexes
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private bool ValidateIndexes(string fieldName, string value)
    {
        var matches = _stringInterpolationArgsRegex.Matches(value);
        if (matches.Any())
        {
            HashSet<int> sortedNumbers = new HashSet<int>();

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    if (!sortedNumbers.Add(int.Parse(match.Groups[1].Value)))
                    {
                        Assert.Fail($"{fieldName} interpolation indexes must be unique.");
                        return false;
                    }
                }
            }

            var maxIndex = sortedNumbers.Count - 1;
            if (sortedNumbers.Any(number => number > maxIndex))
            {
                Assert.Fail($"{fieldName} has one or more out of range indexes. Max index should be {maxIndex}.");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Display the failed message for <see cref="_openBraceWithoutCloseRegex" /> and 
    /// <see cref="_closeBraceWithoutOpenRegex" />
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="curlyBraceType"></param>
    private void AssertFailInterpolationCurlyBrace(string fieldName, string curlyBraceType)
        => Assert.Fail($"{fieldName} has a missing {curlyBraceType} interpolation curly brace.");

    /// <summary>
    /// Display the message when the previous message failed, abandoning the current test
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="curlyBraceType"></param>
    private void HandlePreviousTestFailed(string currentTestName)
        => Assert.Fail($"{currentTestName} abandoned. Previous test failed.");
}

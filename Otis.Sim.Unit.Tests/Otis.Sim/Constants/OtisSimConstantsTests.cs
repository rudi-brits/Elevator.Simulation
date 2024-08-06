using Otis.Sim.Constants;
using Otis.Sim.Unit.Tests.Constants;
using Otis.Sim.Unit.Tests.TestBase.Services;
using System.Reflection;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Constants;

/// <summary>
/// Class OtisSimConstantsTests.
/// </summary>
/// <category><see cref="TestConstants.Constants" /></category>
[TestFixture(Category = TestConstants.Constants)]
public class OtisSimConstantsTests : BaseTestService
{
    /// <summary>
    /// A list of all constant string properties of the <see cref="OtisSimConstants" /> class
    /// </summary>
    IEnumerable<FieldInfo> _fieldInfo;

    /// <summary>
    /// Setup before any tests are run.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _fieldInfo = GetPublicStaticLiteralFieldsInfo<OtisSimConstants>();
    }

    /// <summary>
    /// Ensure the class has constants.
    /// </summary>
    [Test]
    public void ConstantFields_Exists()
    {
        Assert.That(_fieldInfo, Is.Not.Null.Or.Empty);
    }

    /// <summary>
    /// Ensure that all constants have string values that are not null or whitespace.
    /// </summary>
    [Test]
    public void ConstantFieldValues_AreNotNullOrWhitespace()
    {
        foreach (var simConstants in _fieldInfo)
        {
            var constantValue = $"{simConstants.GetRawConstantValue()}";
            if (string.IsNullOrWhiteSpace(constantValue))
            {
                Assert.Fail($"{simConstants.Name} {TestConstants.HasAnEmptyValue}");
                return;
            }
        }
    }
}

using Otis.Sim.Unit.Tests.Constants;
using Otis.Sim.Unit.Tests.Otis.Sim.MappingProfiles.MockClasses;
using Otis.Sim.Unit.Tests.TestBase.Services;
using System.Reflection;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Services;

/// <summary>
/// Class OtisMappingProfileTests extends the <see cref="BaseTestService" /> class.
/// </summary>
/// <category><see cref="TestConstants.SimulationServiceCategory" /></category>
public class OtisSimulationServiceTests : BaseTestService
{
    /// <summary>
    /// OtisSimulationServiceMock field
    /// </summary>
    private OtisSimulationServiceMock _otisSimulationServiceMock;

    /// <summary>
    /// OneTimeSetUp of mocks and services.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _otisSimulationServiceMock = new OtisSimulationServiceMock();
    }

    /// <summary>
    /// Test constructor
    /// </summary>
    [Test]
    public void Constructor_WithExpected_InitialisationsAndDefaults()
    {
        var serviceProviderField = GetServiceProviderField();
        ValidateFieldValue(serviceProviderField, _otisSimulationServiceMock, false);

        var successPrefixValue = GetSuccessPrefixValue();
        Assert.That(successPrefixValue, Is.Not.Null.Or.Empty);
    }

    /// <summary>
    /// SetupServiceCollection expected result
    /// </summary>
    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public void SetupServiceCollection_ExpectedResult(bool throwException)
    {
        var methodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>("SetupServiceCollection");
        methodInfo.Invoke(_otisSimulationServiceMock, null);

        _otisSimulationServiceMock.ThrowWriteLineException = throwException;

        var messageMethodInfo = GetNonPublicInstanceMethodNotNull<OtisSimulationServiceMock>("SetupServiceCollectionMessage");
        var message = messageMethodInfo.Invoke(_otisSimulationServiceMock, new object[] { !throwException }) as string ?? "";

        Assert.That(_otisSimulationServiceMock.CalledWriteLineToConsole, Is.True);
        if (!throwException)
            Assert.That(_otisSimulationServiceMock.WriteLineToConsoleValue, Is.EqualTo(message));
        else
            Assert.That(_otisSimulationServiceMock.WriteLineToConsoleValue?.Contains(message), Is.True);
    }

    /// <summary>
    /// GetSuccessPrefixValue
    /// </summary>
    /// <returns></returns>
    private object? GetSuccessPrefixValue()
    {
        var successPrefixField 
            = GetNonPublicInstanceFieldNotNull<OtisSimulationServiceMock>("SuccessPrefix");
        return ValidateFieldValue(successPrefixField, _otisSimulationServiceMock);
    }

    /// <summary>
    /// GetServiceProviderField
    /// </summary>
    /// <returns></returns>
    private FieldInfo GetServiceProviderField()
        => GetNonPublicInstanceFieldNotNull<OtisSimulationServiceMock>("_serviceProvider");

    /// <summary>
    /// GetServiceProviderValue
    /// </summary>
    /// <returns></returns>
    private object? GetServiceProviderValue()
    {
        var serviceProviderField = GetServiceProviderField();
        return ValidateFieldValue(serviceProviderField, _otisSimulationServiceMock);
    }
}
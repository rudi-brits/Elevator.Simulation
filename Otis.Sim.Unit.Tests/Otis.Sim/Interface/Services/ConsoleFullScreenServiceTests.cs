using Otis.Sim.Interface.Services;
using Otis.Sim.Unit.Tests.Constants;
using System.Reflection;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Interface.Services;

/// <summary>
/// Class ConsoleFullScreenServiceTests extends the <see cref="InterfaceTests" /> class.
/// </summary>
public class ConsoleFullScreenServiceTests : InterfaceTests
{
    /// <summary>
    /// FieldInfo[] of private static fields.
    /// </summary>
    private FieldInfo[] _fieldInfo;

    /// <summary>
    /// Class constructor.
    /// </summary>
    public ConsoleFullScreenServiceTests()
    {
        _fieldInfo = GetNonPublicStaticFields<ConsoleFullScreenService>();
    }

    /// <summary>
    /// Ensure private static fields exists.
    /// </summary>
    [Test]
    public void HasPrivateFields()
    {
        Assert.That(_fieldInfo, Is.Not.Null.And.Not.Empty);
    }

    /// <summary>
    /// Ensure all private static fields have values.
    /// </summary>
    [Test]
    public void AllPrivateFields_HaveValues()
    {
        foreach (var field in _fieldInfo)
        {
            var value = field.GetValue(null);
            if (value == null)
            {
                Assert.Fail($"{field.Name} {TestConstants.HasAnEmptyValue}");
            }
        }
    }

    /// <summary>
    /// Ensure InitialiseFullScreen is called without errors.
    /// </summary>
    [Test]
    public void InitialiseFullScreen_NoError()
    {
        ConsoleFullScreenService consoleFullScreenService = new ConsoleFullScreenService();

        var methodInfo = typeof(ConsoleFullScreenService)
                .GetMethod("InitialiseFullScreen", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(methodInfo, Is.Not.Null);

        Assert.DoesNotThrow(() =>
        {
            methodInfo.Invoke(consoleFullScreenService, null);
        });
    }
}

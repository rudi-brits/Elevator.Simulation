using Otis.Sim.Interface.Constants;
using Otis.Sim.Unit.Tests.Constants;
using System.Reflection;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Interface.Constants;

/// <summary>
/// Class TerminalUiConstantsTests extends the <see cref="InterfaceTests" /> class.
/// </summary>
public class TerminalUiConstantsTests : InterfaceTests
{
    /// <summary>
    /// A list of static PropertyInfo.
    /// </summary>
    PropertyInfo[] _staticProperties;

    /// <summary>
    /// Setup before any tests are run.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _staticProperties = GetPublicStaticProperties<TerminalUiConstants>();
    }

    /// <summary>
    /// Has static properties.
    /// </summary>
    [Test]
    public void StaticProperties_IsNotNullOrEmpty()
    {
        Assert.That(_staticProperties, Is.Not.Null.Or.Empty);
    }

    /// <summary>
    /// Static properties all have values.
    /// </summary>
    [Test]
    public void StaticProperties_AllHaveValues()
    {
        foreach(var property in _staticProperties)
        {
            var value = property.GetValue(null) as Attribute;
            if (value == null)
            {
                Assert.Fail($"{property.Name} {TestConstants.HasAnEmptyValue}");
                return;
            }
        }
    }
}

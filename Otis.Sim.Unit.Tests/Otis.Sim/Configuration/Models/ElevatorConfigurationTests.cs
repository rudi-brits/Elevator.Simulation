using Otis.Sim.Configuration.Models;

namespace Otis.Sim.Unit.Tests.Configuration.Models;

/// <summary>
/// Class ElevatorConfigurationTests extends the <see cref="ConfigurationTests" /> class.
/// </summary>
public class ElevatorConfigurationTests : ConfigurationTests
{
    /// <summary>
    /// Validate the existence of all inherited properties.
    /// Refer to <see cref="ElevatorConfigurationBaseTests" /> for inherited property values tests.
    /// </summary>
    [Test]
    public void Test_InheritedProperties()
    {
        var basePropertyNames = GetPropertyNames<ElevatorConfigurationBase>();
        Assert.That(basePropertyNames, Is.Not.Null.Or.Empty);

        var derivedPropertyNames = GetPropertyNames<ElevatorConfiguration>();
        Assert.That(derivedPropertyNames, Is.Not.Null.Or.Empty);

        Assert.IsTrue(basePropertyNames.All(derivedPropertyNames.Contains),
            "Not all base properties are present in the derived class.");
    }
}

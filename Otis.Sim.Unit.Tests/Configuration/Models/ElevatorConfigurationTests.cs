using Otis.Sim.Configuration.Models;
using System.Reflection;

namespace Otis.Sim.Unit.Tests.Configuration.Models;

/// <summary>
/// Class ElevatorConfigurationTests, extends the <see cref="ConfigurationTests" /> class.
/// </summary>
public class ElevatorConfigurationTests : ConfigurationTests
{
    [Test]
    public void ValidateInheritedProperties()
    {
        var basePropertyNames = GetPropertyNames<ElevatorConfigurationBase>();
        Assert.That(basePropertyNames, Is.Not.Null.Or.Empty);

        var derivedPropertyNames = GetPropertyNames<ElevatorConfiguration>();
        Assert.That(derivedPropertyNames, Is.Not.Null.Or.Empty);

        Assert.IsTrue(basePropertyNames.All(derivedPropertyNames.Contains),
            "Not all base properties are present in the derived class.");
    }
}

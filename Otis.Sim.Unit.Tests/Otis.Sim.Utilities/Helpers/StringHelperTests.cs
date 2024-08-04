using Otis.Sim.Utilities.Helpers;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Utilities.Helpers;

/// <summary>
/// Class StringHelperTests, extends the <see cref="ExtensionsTests" /> class.
/// </summary>
public class StringHelperTests
{
    /// <summary>
    /// Test that the list of formatted property names are not empty
    /// </summary>
    [Test]
    [TestCase("PostalAdressSecondary", "Postal Adress Secondary")]
    [TestCase("PostalAdress_Secondary", "Postal Adress_Secondary")]
    [TestCase("Age", "Age")]
    [TestCase("Thiswillbeaverylogpieceoftext", "Thiswillbeaverylogpieceoftext")]
    public void SplitCamelCase(string input, string expectedValue)
    {
        var result = StringHelper.SplitCamelCase(input);
        Assert.That(result, Is.EqualTo(expectedValue));
    }
}

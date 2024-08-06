using Otis.Sim.Unit.Tests.Constants;
using Otis.Sim.Utilities.Constants;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Utilities.Constants;

/// <summary>
/// Class UtilityConstantsTests.
/// </summary>
/// <category><see cref="TestConstants.UtilitiesConstantsCategory" /></category>
[TestFixture(Category = TestConstants.UtilitiesConstantsCategory)]
public class UtilityConstantsTests
{
    [Test]
    public void NewLineCharacter_HasValue()
    {
        var result = UtilityConstants.NewLineCharacter;
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }
}

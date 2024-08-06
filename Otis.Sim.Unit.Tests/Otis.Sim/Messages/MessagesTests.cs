using Otis.Sim.Unit.Tests.Constants;
using Otis.Sim.Unit.Tests.TestBase.Services;
using System.Text.RegularExpressions;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Messages;

/// <summary>
/// Class MessagesTests extends the <see cref="BaseTestService" /> class.
/// </summary>
/// <category><see cref="TestConstants.MessagesCategory" /></category>
[TestFixture(Category = TestConstants.MessagesCategory)]
public abstract class MessagesTests : BaseTestService
{
    protected readonly Regex _stringInterpolationArgsRegex = new Regex(@"\{(\d+)\}", RegexOptions.Compiled);
}

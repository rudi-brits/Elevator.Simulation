using Otis.Sim.Unit.Tests.Constants;
using System.Text.RegularExpressions;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Messages;

/// <summary>
/// Class MessagesTests.
/// </summary>
/// <category><see cref="TestConstants.MessagesCategory" /></category>
[TestFixture(Category = TestConstants.MessagesCategory)]
public abstract class MessagesTests
{
    protected readonly Regex _stringInterpolationArgsRegex = new Regex(@"\{(\d+)\}", RegexOptions.Compiled);
}

using Moq;
using Otis.Sim.Interface.Interfaces;
using Terminal.Gui;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Interface.Models;

/// <summary>
/// Class SimTerminalGuiApplicationTests extends the <see cref="InterfaceTests" /> class.
/// </summary>
public class SimTerminalGuiApplicationTests : InterfaceTests
{
    /// <summary>
    /// The Mock<ISimTerminalGuiApplication> field.
    /// </summary>
    private Mock<ISimTerminalGuiApplication> _mockSimTerminalGuiApplication;

    /// <summary>
    /// SetUp before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockSimTerminalGuiApplication = new Mock<ISimTerminalGuiApplication>();
        _mockSimTerminalGuiApplication.Setup(x => x.Init()).Verifiable();
        _mockSimTerminalGuiApplication.Setup(x => x.Top).Returns(new Toplevel());
        _mockSimTerminalGuiApplication.Setup(x => x.Run()).Verifiable();
    }

    /// <summary>
    /// Test Init method invocation.
    /// </summary>
    [Test]
    public void ShouldInvoke_InitMethod()
    {
        var application = _mockSimTerminalGuiApplication.Object;
        application.Init();
        _mockSimTerminalGuiApplication.Verify(x => x.Init(), Times.Once);
    }

    /// <summary>
    /// Test Init method invocation.
    /// </summary>
    [Test]
    public void ShouldReturn_Toplevel()
    {
        var application = _mockSimTerminalGuiApplication.Object;
        var topLevel = application.Top;

        Assert.IsNotNull(topLevel);
        Assert.IsInstanceOf<Toplevel>(topLevel);
    }

    /// <summary>
    /// Test Run method invocation.
    /// </summary>
    [Test]
    public void ShouldInvoke_RunMethod()
    {
        var application = _mockSimTerminalGuiApplication.Object;
        application.Run();
        _mockSimTerminalGuiApplication.Verify(x => x.Run(), Times.Once);
    }
}

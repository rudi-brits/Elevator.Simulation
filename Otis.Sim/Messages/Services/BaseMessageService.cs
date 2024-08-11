namespace Otis.Sim.Messages.Services;

/// <summary>
/// BaseMessageService
/// </summary>
public class BaseMessageService
{
    /// <summary>
    /// FormatMessage
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inputs"></param>
    /// <returns>The string result</returns>
    public static string FormatMessage(string message, params string[] inputs)
        => string.Format(message, inputs);
}

namespace Otis.Sim.Messages.Services
{
    public class BaseMessageService
    {
        public static string FormatMessage(string message, params string[] inputs)
        {
            return string.Format(message, inputs);
        }
    }
}

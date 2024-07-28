namespace Otis.Sim.Messages.Services
{
    public class ValidationMessageService : BaseMessageService
    {
        public const string LargerThanAnotherValueMessage = "{0} must be a larger than {1}";
        public const string LargerThanOrEqualAnotherValueMessage = "{0} must be a larger than or equal to {1}";
        public const string LessThanOrEqualAnotherValueMessage = "{0} must be a less than or equal to {1}";
        public const string MayNotBeNullOrEmpty = "{0} may not be null or empty";
    }
}

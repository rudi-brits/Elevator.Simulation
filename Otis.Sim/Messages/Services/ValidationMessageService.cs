namespace Otis.Sim.Messages.Services
{
    public class ValidationMessageService : BaseMessageService
    {
        public const string MustBeLargerThanOtherValue = "{0} must be a larger than {1}";
        public const string MustBeLargerThanOrEqualToOtherValue = "{0} must be a larger than or equal to {1}";
        public const string MustBeLessThanOrEqualToOtherValue = "{0} must be a less than or equal to {1}";
        public const string MayNotBeNull = "{0} may not be null";
        public const string MayNotBeEmpty = "{0} may not be empty";
        public const string MayNotBeNullOrEmpty = "{0} may not be null or empty";
    }
}

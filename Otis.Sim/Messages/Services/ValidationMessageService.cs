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
        public const string MustBeValidInteger = "{0} must be a valid integer";
        public const string MustBeWithinRange = "The {0} must be between {1} and {2}.";
        public const string MayNotBeEqualTo = "{0} may not be equal to {1}";
        public const string DuplicateElevatorRequest = "A request from {0} to {1} going {2} is already pending.";
    }
}

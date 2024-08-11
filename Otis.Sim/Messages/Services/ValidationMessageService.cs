namespace Otis.Sim.Messages.Services;

/// <summary>
/// Class ValidationMessageService extends the <see cref="BaseMessageService" /> class.
/// </summary>
public class ValidationMessageService : BaseMessageService
{
    /// <summary>
    /// MustBeLargerThanOtherValue
    /// </summary>
    public const string MustBeLargerThanOtherValue = "{0} must be a larger than {1}";
    /// <summary>
    /// MustBeLargerThanOrEqualToOtherValue
    /// </summary>
    public const string MustBeLargerThanOrEqualToOtherValue = "{0} must be a larger than or equal to {1}";
    /// <summary>
    /// MustBeLessThanOrEqualToOtherValue
    /// </summary>
    public const string MustBeLessThanOrEqualToOtherValue = "{0} must be a less than or equal to {1}";
    /// <summary>
    /// MayNotBeNull
    /// </summary>
    public const string MayNotBeNull = "{0} may not be null";
    /// <summary>
    /// MayNotBeEmpty
    /// </summary>
    public const string MayNotBeEmpty = "{0} may not be empty";
    /// <summary>
    /// MayNotBeNullOrEmpty
    /// </summary>
    public const string MayNotBeNullOrEmpty = "{0} may not be null or empty";
    /// <summary>
    /// MustBeValidInteger
    /// </summary>
    public const string MustBeValidInteger = "{0} must be a valid integer";
    /// <summary>
    /// MustBeWithinRange
    /// </summary>
    public const string MustBeWithinRange = "The {0} must be between {1} and {2}.";
    /// <summary>
    /// MayNotBeEqualTo
    /// </summary>
    public const string MayNotBeEqualTo = "{0} may not be equal to {1}";
    /// <summary>
    /// DuplicateElevatorRequest
    /// </summary>
    public const string DuplicateElevatorRequest = "A request from {0} to {1} going {2} is already pending.";
    /// <summary>
    /// MayNotContainDuplicateValues
    /// </summary>
    public const string MayNotContainDuplicateValues = "Duplicate {0} values are not allowed.";
}

using FluentValidation;
using Otis.Sim.Constants;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Utilities.Extensions;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Elevator.Validators;

/// <summary>
/// ElevatorRequestValidator class extends <see cref="AbstractValidator<>" /> of type <see cref="ElevatorRequest<>" />
/// </summary>
public class ElevatorRequestValidator : AbstractValidator<ElevatorRequest>
{
    /// <summary>
    /// ElevatorRequestValidator constructor
    /// </summary>
    /// <param name="validationValues"></param>
    public ElevatorRequestValidator(ElevatorRequestValidationValues validationValues)
    {
        RuleFor(model => model.OriginFloor)
            .Must((model, originFloor) => originFloor != model.DestinationFloor)
                .WithMessage(MessageService.FormatMessage(
                    MessageService.MayNotBeEqualTo,
                    OtisSimConstants.OriginFloorName,
                    OtisSimConstants.DestinationFloorName))
            .Must(value => value.IsInRange(validationValues.LowestFloor, validationValues.HighestFloor))
                .WithMessage(MessageService.FormatMessage(
                    MessageService.MustBeWithinRange,
                    OtisSimConstants.OriginFloorName,
                    $"{validationValues.LowestFloor}",
                    $"{validationValues.HighestFloor}"));

        RuleFor(model => model.DestinationFloor)
            .Must(value => value.IsInRange(validationValues.LowestFloor, validationValues.HighestFloor))
                .WithMessage(MessageService.FormatMessage(
                    MessageService.MustBeWithinRange,
                    OtisSimConstants.DestinationFloorName,
                    $"{validationValues.LowestFloor}",
                    $"{validationValues.HighestFloor}"));

        RuleFor(model => model.NumberOfPeople)
            .Must(value => value.IsInRange(1, validationValues.MaximumLoad))
                .WithMessage(MessageService.FormatMessage(
                    MessageService.MustBeWithinRange,
                    OtisSimConstants.NumberOfPeopleName,
                    $"{1}",
                    $"{validationValues.MaximumLoad}"));
    }
}

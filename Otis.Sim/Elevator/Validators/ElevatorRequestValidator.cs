using FluentValidation;
using Otis.Sim.Elevator.Models;
using UiConstants = Otis.Sim.Interface.Constants.TerminalUiConstants;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;
using Otis.Sim.Utilities.Extensions;

namespace Otis.Sim.Elevator.Validators
{
    public class ElevatorRequestValidator : AbstractValidator<ElevatorRequest>
    {
        public ElevatorRequestValidator(ElevatorRequestValidationValues validationValues)
        {
            RuleFor(model => model.OriginFloor)
                .Must((model, originFloor) => originFloor != model.DestinationFloor)
                    .WithMessage(Messages.Services.BaseMessageService.FormatMessage(
                        MessageService.MayNotBeEqualTo,
                        UiConstants.OriginFloorName,
                        UiConstants.DestinationFloorName))
                .Must(value => value.IsInRange(validationValues.LowestFloor, validationValues.HighestFloor))
                    .WithMessage(Messages.Services.BaseMessageService.FormatMessage(
                        MessageService.MustBeWithinRange,
                        UiConstants.OriginFloorName,
                        $"{validationValues.LowestFloor}",
                        $"{validationValues.HighestFloor}"));

            RuleFor(model => model.DestinationFloor)
                .Must(value => value.IsInRange(validationValues.LowestFloor, validationValues.HighestFloor))
                    .WithMessage(Messages.Services.BaseMessageService.FormatMessage(
                        MessageService.MustBeWithinRange,
                        UiConstants.DestinationFloorName,
                        $"{validationValues.LowestFloor}",
                        $"{validationValues.HighestFloor}"));

            RuleFor(model => model.Capacity)
                .Must(value => value.IsInRange(1, validationValues.MaximumLoad))
                    .WithMessage(Messages.Services.BaseMessageService.FormatMessage(
                        MessageService.MustBeWithinRange,
                        UiConstants.NumberOfPeopleName,
                        $"{1}",
                        $"{validationValues.MaximumLoad}"));
        }
    }
}

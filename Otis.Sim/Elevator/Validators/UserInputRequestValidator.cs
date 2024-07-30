using FluentValidation;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Interface.Validators.Helpers;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;
using UiConstants = Otis.Sim.Interface.Constants.TerminalUiConstants;

namespace Otis.Sim.Elevator.Validators
{
    public class UserInputRequestValidator : AbstractValidator<UserInputRequest>
    {
        public UserInputRequestValidator()
        {
            RuleFor(model => model.OriginFloorInput)
                .Must(value => UStringHelper.ToInteger(value).HasValue)
                    .WithMessage(Messages.Services.BaseMessageService.FormatMessage(
                        MessageService.MustBeValidInteger,
                        UiConstants.OriginFloorName));

            RuleFor(model => model.DestinationFloorInput)
                .Must(value => UStringHelper.ToInteger(value).HasValue)
                    .WithMessage(Messages.Services.BaseMessageService.FormatMessage(
                        MessageService.MustBeValidInteger,
                        UiConstants.DestinationFloorName));

            RuleFor(model => model.CapacityInput)
                .Must(value => UStringHelper.ToInteger(value).HasValue)
                    .WithMessage(Messages.Services.BaseMessageService.FormatMessage(
                        MessageService.MustBeValidInteger,
                        UiConstants.NumberOfPeopleName));
        }
    }
}

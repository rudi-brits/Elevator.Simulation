using FluentValidation;
using Otis.Sim.Constants;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Interface.Validators.Helpers;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

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
                        OtisSimConstants.OriginFloorName));

            RuleFor(model => model.DestinationFloorInput)
                .Must(value => UStringHelper.ToInteger(value).HasValue)
                    .WithMessage(Messages.Services.BaseMessageService.FormatMessage(
                        MessageService.MustBeValidInteger,
                        OtisSimConstants.DestinationFloorName));

            RuleFor(model => model.CapacityInput)
                .Must(value => UStringHelper.ToInteger(value).HasValue)
                    .WithMessage(Messages.Services.BaseMessageService.FormatMessage(
                        MessageService.MustBeValidInteger,
                        OtisSimConstants.NumberOfPeopleName));
        }
    }
}

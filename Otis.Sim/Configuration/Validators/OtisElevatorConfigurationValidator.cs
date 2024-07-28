using FluentValidation;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Utilities.Extensions;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Configuration.Validators
{
    public class OtisElevatorConfigurationValidator : AbstractValidator<OtisElevatorConfiguration>
    {
        private const string Description = "Elevator Configuration - ";

        public OtisElevatorConfigurationValidator(OtisElevatorConfiguration elevatorConfiguration)
        {
            RuleFor(model => model.Description)
               .Must(value => string.IsNullOrWhiteSpace(value))
                  .WithMessage($"{Description}" +
                     MessageService.FormatMessage(MessageService.MayNotBeNullOrEmpty, "description"));

            RuleFor(model => model.HighestFloor)
               .Must(value => ValidateFloorDifference(value, elevatorConfiguration.LowestFloor))
                  .WithMessage($"{Description} {elevatorConfiguration.Description} " +
                     MessageService.FormatMessage(MessageService.LessThanOrEqualAnotherValueMessage,
                        "highest floor", "lowest floor"));
        }

        private bool ValidateFloorDifference(int? highestFloor, int? lowestFloor)
        {
            if (highestFloor.HasValue && lowestFloor.HasValue)
                return ((int)highestFloor).LargerThanMinimumDifference((int)lowestFloor);

            return true;
        }
    }
}

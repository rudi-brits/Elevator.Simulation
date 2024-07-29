using FluentValidation;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Utilities.Extensions;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Configuration.Validators
{
    public class ElevatorConfigurationValidator : AbstractValidator<ElevatorConfiguration>
    {
        const string modelPrefix = $"{nameof(ElevatorConfiguration)} - ";

        public ElevatorConfigurationValidator()
        {
            RuleFor(model => model.Description)
               .NotEmpty()
                  .WithMessage(model => $"{modelPrefix}" +
                     MessageService.FormatMessage(
                         MessageService.MayNotBeNullOrEmpty,
                         nameof(model.Description)));

            RuleFor(model => model.HighestFloor)
              .Must((model, highestFloor) => ValidateFloorDifference(highestFloor, model.LowestFloor))
                 .WithMessage(model => $"{modelPrefix}{model.Description} " +
                    MessageService.FormatMessage(
                        MessageService.MustBeLargerThanOtherValue,
                        nameof(model.HighestFloor),
                        nameof(model.LowestFloor)));
        }

        private bool ValidateFloorDifference(int? highestFloor, int? lowestFloor)
        {
            if (highestFloor.HasValue && lowestFloor.HasValue)
                return ((int)highestFloor).LargerThanByDifference((int)lowestFloor);

            return true;
        }
    }
}

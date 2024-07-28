using FluentValidation;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Utilities.Extensions;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Configuration.Validators
{
    public class OtisBuildingConfigurationValidator : AbstractValidator<OtisBuildingConfiguration>
    {
        private const string Description = "Building Configuration - ";

        public OtisBuildingConfigurationValidator(OtisBuildingConfiguration buildingConfiguration)
        {
            RuleFor(model => model.HighestFloor)
                .Must(value => value.LargerThanMinimumDifference(buildingConfiguration.LowestFloor))
                .WithMessage($"{Description}" +
                    MessageService.FormatMessage(MessageService.LargerThanAnotherValueMessage,
                        "highest floor", "lowest floor"));

            RuleFor(model => model.MaximumElevatorLoad)
               .Must(value => value > 0)
               .WithMessage($"{Description} elevator load must be larger than zero");
        }
    }
}

using FluentValidation;
using Otis.Sim.Configuration.Models;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Configuration.Validators
{
    public class OtisConfigurationValidator : AbstractValidator<OtisConfiguration>
    {
        public OtisConfigurationValidator()
        {
            RuleFor(config => config.BuildingConfiguration)
                .NotNull()
                    .WithMessage(model => MessageService.FormatMessage(
                        MessageService.MayNotBeNull,
                        nameof(model.BuildingConfiguration)))
                .SetValidator(new BuildingConfigurationValidator());

            RuleFor(config => config.ElevatorsConfiguration)
                .NotNull()
                    .WithMessage(model => MessageService.FormatMessage(
                        MessageService.MayNotBeNull,
                        nameof(model.ElevatorsConfiguration)))
                .NotEmpty()
                    .WithMessage(model => MessageService.FormatMessage(
                        MessageService.MayNotBeEmpty,
                        nameof(model.ElevatorsConfiguration)));

            RuleForEach(config => config.ElevatorsConfiguration)
                .SetValidator(new ElevatorConfigurationValidator());
        }
    }
}

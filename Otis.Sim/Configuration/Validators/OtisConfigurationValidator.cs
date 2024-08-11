using FluentValidation;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Constants;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Configuration.Validators;

/// <summary>
/// OtisConfigurationValidator extends the <see cref="AbstractValidator" /> class.
/// </summary>
public class OtisConfigurationValidator : AbstractValidator<OtisConfiguration>
{
    /// <summary>
    /// OtisConfigurationValidator constructor
    /// </summary>
    public OtisConfigurationValidator()
    {
        RuleFor(config => config.BuildingConfiguration)
            .NotNull()
                .WithMessage(model => MessageService.FormatMessage(
                    MessageService.MayNotBeNull,
                    nameof(model.BuildingConfiguration)))
            .SetValidator(new BuildingConfigurationValidator()!);

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

        RuleFor(config => config.ElevatorsConfiguration)
            .Must(NoDuplicateDescriptions)
                .WithMessage(model => MessageService.FormatMessage(
                    MessageService.MayNotContainDuplicateValues,
                    $"{OtisSimConstants.Elevator} {nameof(ElevatorConfiguration.Description)}"));
    }

    /// <summary>
    /// NoDuplicateDescriptions
    /// </summary>
    /// <param name="elevators"></param>
    /// <returns>The boolean result</returns>
    private bool NoDuplicateDescriptions(List<ElevatorConfiguration>? elevators)
    {
        if (elevators != null && elevators.Any())
        {
            var descriptions = elevators.Select(e => (e.Description ?? "").Trim()).ToList();
            return descriptions.Distinct().Count() == descriptions.Count;
        }   
        return true;
    }
}

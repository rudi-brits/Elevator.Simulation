﻿using FluentValidation;
using Otis.Sim.Configuration.Models;
using Otis.Sim.Constants;
using Otis.Sim.Utilities.Extensions;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Configuration.Validators;

/// <summary>
/// BuildingConfigurationValidator extends the <see cref="AbstractValidator" /> class.
/// </summary>
public class BuildingConfigurationValidator : AbstractValidator<BuildingConfiguration>
{
    /// <summary>
    /// BuildingConfigurationValidator constructor
    /// </summary>
    public BuildingConfigurationValidator()
    {
        const string modelPrefix = $"{nameof(BuildingConfiguration)} - ";

        RuleFor(model => model.HighestFloor)
            .Must((model, highestFloor) => highestFloor.LargerThanByDifference(model.LowestFloor))
                .WithMessage(model => $"{modelPrefix}" + 
                    MessageService.FormatMessage(
                        MessageService.MustBeLargerThanOtherValue,
                        nameof(model.HighestFloor), 
                        nameof(model.LowestFloor)));

        RuleFor(model => model.MaximumElevatorLoad)
            .GreaterThan(0)
                .WithMessage(model => $"{modelPrefix}" +
                    MessageService.FormatMessage(
                        MessageService.MustBeLargerThanOtherValue,
                        nameof(model.MaximumElevatorLoad),
                        OtisSimConstants.Zero));
    }
}

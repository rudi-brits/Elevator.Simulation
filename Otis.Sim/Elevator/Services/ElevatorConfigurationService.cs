﻿using AutoMapper;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Utilities.Extensions;

namespace Otis.Sim.Elevator.Services
{
    public abstract class ElevatorConfigurationService
    {
        protected List<ElevatorModel> _elevators = new List<ElevatorModel>();
        protected ElevatorRequestValidationValues _elevatorRequestValidationValues;

        protected OtisConfigurationService _configurationService;
        protected IMapper _mapper;

        protected ElevatorConfigurationService(OtisConfigurationService configurationService,
            IMapper mapper)
        {
            _configurationService = configurationService;
            _mapper = mapper;
        }

        public void LoadConfiguration()
        {
            var elevatorId = 0;
            var buildingConfiguration = _configurationService.BuildingConfiguration!;

            _configurationService.ElevatorsConfiguration!.ForEach(elevatorConfiguration =>
            {
                _elevators.Add(new ElevatorModel(_mapper)
                {
                    Id                 = ++elevatorId,
                    Description        = elevatorConfiguration.Description.Trim(),
                    LowestFloor        = elevatorConfiguration.LowestFloor.ApplyHigherValue(buildingConfiguration.LowestFloor),
                    HighestFloor       = elevatorConfiguration.HighestFloor.ApplyLowerValue(buildingConfiguration.HighestFloor),
                    MaximumLoad        = buildingConfiguration.MaximumElevatorLoad,
                    CompleteRequest    = CompleteRequest,
                    PrintRequestStatus = PrintRequestStatus
                });
            });

            _elevatorRequestValidationValues = new ElevatorRequestValidationValues()
            {
                LowestFloor = _elevators.Min(elevator => elevator.LowestFloor),
                HighestFloor = _elevators.Max(elevator => elevator.HighestFloor),
                MaximumLoad = buildingConfiguration.MaximumElevatorLoad
            };

            PrintLoadedConfiguration();
        }

        protected abstract void CompleteRequest(Guid requestId);
        protected abstract void PrintRequestStatus(string message);

        private void PrintLoadedConfiguration()
        {
            Console.WriteLine();

            Console.WriteLine(
                "NOTE: Elevator configuration may be overridden by building configuration where values exceed the bounds of the building");
            Console.WriteLine();

            Console.WriteLine("Building configuration:");
            Console.WriteLine(_configurationService.BuildingConfiguration!.ToString());
            Console.WriteLine();

            Console.WriteLine("Elevators configuration:");
            _elevators.ForEach(elevator => Console.WriteLine(elevator.ToString()));

            Console.WriteLine();
        }
    }
}

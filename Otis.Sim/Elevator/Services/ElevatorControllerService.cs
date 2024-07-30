using AutoMapper;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Elevator.Validators;
using Otis.Sim.Utilities.Helpers;

namespace Otis.Sim.Elevator.Services
{
    public class ElevatorControllerService : ElevatorConfigurationService
    {
        private List<string> _elevatorTableHeaders;
        public List<string> ElevatorTableHeaders => _elevatorTableHeaders;
        public List<ElevatorDataRow> ElevatorDataRows => _mapper.Map<List<ElevatorDataRow>>(_elevators);
        public string StatusFieldName => nameof(ElevatorDataRow.Status);

        private readonly object _lockRequestQueue;
        private HashSet<Guid> _completedRequestIds;

        private readonly IMapper _mapper;

        public ElevatorControllerService(OtisConfigurationService configurationService,
            IMapper mapper): base(configurationService)
        {   
            _mapper = mapper;
            _elevatorTableHeaders = ReflectionHelper.GetFormattedPropertyNames<ElevatorDataRow>();
        }
        
        public ElevatorRequestResult RequestElevator(UserInputRequest userInputRequest)
        {
            var userInputValidationResult = ValidateUserInputRequest(userInputRequest);
            if (userInputValidationResult != null)
                return userInputValidationResult;

            var elevatorRequest = new ElevatorRequest(userInputRequest);

            var elevatorRequestValidationResult = ValidateElevatorRequest(elevatorRequest);
            if (elevatorRequestValidationResult != null)
                return elevatorRequestValidationResult;

            return new ElevatorRequestResult(true);
        }

        private ElevatorRequestResult? ValidateUserInputRequest(UserInputRequest userInputRequest)
        {
            var validationResult = new UserInputRequestValidator()
                .Validate(userInputRequest);

            if (!validationResult.IsValid)
                return new ElevatorRequestResult(validationResult.Errors);

            return null;
        }

        private ElevatorRequestResult? ValidateElevatorRequest(ElevatorRequest elevatorRequest)
        {
            var validationResult = new ElevatorRequestValidator(_elevatorRequestValidationValues)
                .Validate(elevatorRequest);

            if (!validationResult.IsValid)
                return new ElevatorRequestResult(validationResult.Errors);

            return null;
        }

        public override void CompleteRequest(Guid requestId)
        {
            lock (_lockRequestQueue)
            {
                if (!_completedRequestIds.Any(id => id == requestId))
                {
                    _completedRequestIds.Add(requestId);
                }
            }
        }
    }
}

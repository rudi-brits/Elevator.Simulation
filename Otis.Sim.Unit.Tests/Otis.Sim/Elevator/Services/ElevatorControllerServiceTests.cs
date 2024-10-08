﻿using AutoMapper;
using Moq;
using Otis.Sim.Configuration.Services;
using Otis.Sim.Constants;
using Otis.Sim.Elevator.Models;
using Otis.Sim.Elevator.Services;
using Otis.Sim.Unit.Tests.Otis.Sim.Elevator.MockClasses;
using System.Reflection;
using MessageService = Otis.Sim.Messages.Services.ValidationMessageService;

namespace Otis.Sim.Unit.Tests.Otis.Sim.Elevator.Services;

/// <summary>
/// Class ElevatorControllerServiceTests extends the <see cref="ElevatorTests" /> class.
/// </summary>
public class ElevatorControllerServiceTests : ElevatorTests
{
    /// <summary>
    /// Mock<IMapper> field.
    /// </summary>
    private IMapper _mapper;
    /// <summary>
    /// ElevatorControllerServiceMock field
    /// </summary>
    private ElevatorControllerServiceMock _elevatorControllerServiceMock;
    /// <summary>
    /// ElevatorControllerServiceMock field
    /// </summary>
    private Mock<ElevatorControllerService> _mockElevatorControllerService;
    /// <summary>
    /// ElevatorControllerServiceMock field
    /// </summary>
    private Mock<OtisConfigurationService> _mockOtisConfigurationService;
    /// <summary>
    /// _elevators 
    /// </summary>
    private List<ElevatorModel> _elevators;
    /// <summary>
    /// _elevatorRequestValidationValues
    /// </summary>
    private ElevatorRequestValidationValues? _elevatorRequestValidationValues;
    /// <summary>
    /// _userInputRequest
    /// </summary>
    private UserInputRequest _userInputRequest;

    /// <summary>
    /// OneTimeSetUp of mocks and services.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _mapper = SetupIMapper();
        _elevators = new List<ElevatorModel>();

        _elevatorRequestValidationValues = new ElevatorRequestValidationValues()
        {
            LowestFloor = 0,
            HighestFloor = 10,
            MaximumLoad = 10
        };

        for (var i = 0; i < 3; i++)
        {
            var elevatorModelMock = new ElevatorModelMock(_mapper);

            elevatorModelMock.Id = i + 1;
            elevatorModelMock.CurrentFloor = _elevatorRequestValidationValues.HighestFloor;
            elevatorModelMock.CallBaseCanAcceptRequest = false;

            _elevators.Add(elevatorModelMock);
        }       

        _mockOtisConfigurationService = new Mock<OtisConfigurationService>();
        _mockElevatorControllerService = new Mock<ElevatorControllerService>(
            _mockOtisConfigurationService.Object, _mapper);
    }

    /// <summary>
    /// SetUp before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _userInputRequest = new UserInputRequest()
        {
            OriginFloorInput      = "1",
            DestinationFloorInput = "2",
            CapacityInput         = "10"
        };

        _elevatorControllerServiceMock = new ElevatorControllerServiceMock(
            _mockOtisConfigurationService.Object, _mapper);
    }

    /// <summary>
    /// Test constructor
    /// </summary>
    [Test]
    public void Constructor_WithExpected_InitialisationsAndDefaults()
    {
        // ElevatorTableHeaders 
        Assert.That(ElevatorControllerService.ElevatorTableHeaders, Is.Not.Null.Or.Empty);

        var elevatorDataRowPropertyNames = GetPublicPropertyNames<ElevatorDataRow>();
        Assert.That(elevatorDataRowPropertyNames.Count,
            Is.EqualTo(ElevatorControllerService.ElevatorTableHeaders.Count));

        // ElevatorDataRows
        var elevatorsField = GetElevatorsField();
        var elevatorsValue = ValidateFieldValue(elevatorsField, _elevatorControllerServiceMock) as List<ElevatorModel>;
        Assert.That(elevatorsValue, Is.Not.Null);
        Assert.That(elevatorsValue.Any(), Is.False);

        elevatorsField?.SetValue(_elevatorControllerServiceMock, _elevators);

        Assert.That(_elevatorControllerServiceMock.ElevatorDataRows, Is.Not.Null.Or.Empty);
        Assert.That(_elevatorControllerServiceMock.ElevatorDataRows.Count,
            Is.EqualTo(_elevators.Count));

        var elevatorRequestValidationValuesField = GetElevatorRequestValidationValuesField();
        var elevatorRequestValidationValuesValue = 
            ValidateFieldValue(elevatorRequestValidationValuesField, _elevatorControllerServiceMock, false);
        Assert.That(elevatorRequestValidationValuesValue, Is.Null);

        Assert.That(_elevatorControllerServiceMock.StatusFieldName, Is.Not.Null.Or.Empty);

        var requestQueueValue = GetRequestQueueValue() as Queue<ElevatorRequest>;
        Assert.That(requestQueueValue, Is.Not.Null);
        Assert.That(requestQueueValue.Any(), Is.False);

        var completedRequestIdsValue = GetCompletedRequestIdsValue() as HashSet<Guid>;
        Assert.That(completedRequestIdsValue, Is.Not.Null);
        Assert.That(completedRequestIdsValue.Any(), Is.False);

        var lockRequestQueueField =
            GetNonPublicInstanceFieldNotNull<ElevatorControllerServiceMock>("_lockRequestQueue");
        var lockRequestQueueValue = ValidateFieldValue(lockRequestQueueField, _elevatorControllerServiceMock);
        Assert.That(lockRequestQueueValue, Is.Not.Null);

        var cancellationTokenSourceValue = GetCancellationTokenSourceValue();
        Assert.That(cancellationTokenSourceValue, Is.Not.Null);

        var requestConsumerTaskValue = GetRequestConsumerTaskValue();
        Assert.That(requestConsumerTaskValue, Is.Not.Null);

        GetNestedTypeNotNull<ElevatorControllerService>("UpdateRequestStatusDelegate");

        var updateRequestStatusField = GetPublicInstanceFieldNotNull<ElevatorControllerService>("UpdateRequestStatus");
        ValidateFieldValue(updateRequestStatusField, _mockElevatorControllerService.Object, false);
    }

    /// <summary>
    /// RunRequestQueueConsumer with expected result 
    /// </summary>
    [Test]
    public void RunRequestQueueConsumer_ExpectedResult()
    {
        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorControllerServiceMock>("RunRequestQueueConsumer");
        methodInfo.Invoke(_elevatorControllerServiceMock, null);

        Assert.That(_elevatorControllerServiceMock.CalledRunRequestQueueConsumer, Is.True);
    }

    /// <summary>
    /// Exit with expected result 
    /// </summary>
    [Test]
    public void Exit_ExpectedResult()
    {
        var cancellationTokenSourceField = GetCancellationTokenSourceField();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSourceField.SetValue(_elevatorControllerServiceMock, cancellationTokenSource);

        var requestConsumerTaskField = GetRequestConsumerTaskField();
        var task = Task.Run(() =>
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }           
        }, cancellationTokenSource.Token);
        requestConsumerTaskField.SetValue(_elevatorControllerServiceMock, task);

        var taskCanceledExceptionOcurred = false;
        try
        {
            _elevatorControllerServiceMock.Exit();
        }
        catch (AggregateException)
        {
            taskCanceledExceptionOcurred = true;
        }

        var cancellationTokenSourceValue = GetCancellationTokenSourceValue() as CancellationTokenSource;
        Assert.That(cancellationTokenSourceValue, Is.Not.Null);
        Assert.That(cancellationTokenSourceValue.IsCancellationRequested, Is.True);

        var requestConsumerTaskValue = GetRequestConsumerTaskValue() as Task;
        var success = taskCanceledExceptionOcurred && requestConsumerTaskValue == null || 
            requestConsumerTaskValue?.IsCompleted == true;
        Assert.That(success, Is.True);
    }

    /// <summary>
    /// RequestElevator with expected result 
    /// </summary>
    /// <param name="validUserInputRequest"></param>
    /// <param name="validElevatorRequest"></param>
    /// <param name="validateDuplicateElevatorRequest"></param>
    [Test]
    [TestCase(true, false, false)]
    [TestCase(false, true, false)]
    [TestCase(false, false, true)]
    [TestCase(false, false, false)]
    public void RequestElevator_ExpectedResult(bool validUserInputRequest, bool validElevatorRequest, 
        bool validateDuplicateElevatorRequest)
    {
        _elevatorControllerServiceMock.ValidateUserInputRequestReturnValue = validUserInputRequest
            ? new ElevatorRequestResult(true, nameof(validUserInputRequest))
            : null;

        _elevatorControllerServiceMock.ValidateElevatorRequestReturnValue = validElevatorRequest
            ? new ElevatorRequestResult(true, nameof(validElevatorRequest))
            : null;

        _elevatorControllerServiceMock.ValidateDuplicateElevatorRequestReturnValue = validateDuplicateElevatorRequest
            ? new ElevatorRequestResult(true, nameof(validateDuplicateElevatorRequest))
            : null;

        var result = _elevatorControllerServiceMock.RequestElevator(_userInputRequest);

        Assert.That(result, Is.Not.Null);
        Assert.That(_elevatorControllerServiceMock.CalledValidateUserInputRequest, Is.True);

        if (validUserInputRequest)
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo(nameof(validUserInputRequest)));
            Assert.That(_elevatorControllerServiceMock.CalledValidateElevatorRequest, Is.False);
            Assert.That(_elevatorControllerServiceMock.CalledValidateDuplicateElevatorRequest, Is.False);
        }
        else
        {
            if (validElevatorRequest)
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(nameof(validElevatorRequest)));
                Assert.That(_elevatorControllerServiceMock.CalledValidateDuplicateElevatorRequest, Is.False);
            }
            else if (validateDuplicateElevatorRequest)
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(nameof(validateDuplicateElevatorRequest)));
                Assert.That(_elevatorControllerServiceMock.CalledValidateElevatorRequest, Is.True);
            }
            else
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.Null);
                Assert.That(_elevatorControllerServiceMock.CalledValidateElevatorRequest, Is.True);
                Assert.That(_elevatorControllerServiceMock.CalledValidateDuplicateElevatorRequest, Is.True);
            }
        }
    }

    /// <summary>
    /// ValidateUserInputRequest with expected result  
    /// </summary>
    /// <param name="isValidRequest"></param>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ValidateUserInputRequest_ExpectedResult(bool isValidRequest)
    {
        _userInputRequest.OriginFloorInput = isValidRequest ? "1" : "a";
        _elevatorControllerServiceMock.CallBaseValidateUserInputRequest = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorControllerServiceMock>("ValidateUserInputRequest");
        var result = methodInfo.Invoke(_elevatorControllerServiceMock, new object[] { _userInputRequest }) as ElevatorRequestResult;

        if (isValidRequest)
            Assert.That(result, Is.Null);
        else
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.Not.Null.And.Not.Empty);
        }
    }

    /// <summary>
    /// ValidateElevatorRequest with expected result  
    /// </summary>
    /// <param name="isValidRequest"></param>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ValidateElevatorRequest_ExpectedResult(bool isValidRequest)
    {
        _userInputRequest.DestinationFloorInput = isValidRequest
            ? _userInputRequest.DestinationFloorInput
            : _userInputRequest.OriginFloorInput;

        var elevatorRequest = new ElevatorRequest(_userInputRequest);

        _elevatorControllerServiceMock.CallBaseValidateElevatorRequest = true;

        var elevatorRequestValidationValuesField = GetElevatorRequestValidationValuesField();
        elevatorRequestValidationValuesField?.SetValue(_elevatorControllerServiceMock, _elevatorRequestValidationValues);

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorControllerServiceMock>("ValidateElevatorRequest");
        var result = methodInfo.Invoke(_elevatorControllerServiceMock, new object[] { elevatorRequest }) as ElevatorRequestResult;

        if (isValidRequest)
            Assert.That(result, Is.Null);
        else
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.Not.Null.And.Not.Empty);
        }
    }

    /// <summary>
    /// ValidateDuplicateElevatorRequest with expected result  
    /// </summary>
    /// <param name="isDuplicateRequest"></param>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ValidateDuplicateElevatorRequest_ExpectedResult(bool isDuplicateRequest)
    {
        var elevatorRequest = new ElevatorRequest(_userInputRequest);

        var requestQueueField = GetRequestQueueField();
        var requestQueue = new Queue<ElevatorRequest>();

        if (isDuplicateRequest)
            requestQueue.Enqueue(elevatorRequest);

        requestQueueField.SetValue(_elevatorControllerServiceMock, requestQueue);

        _elevatorControllerServiceMock.CallBaseValidateDuplicateElevatorRequest = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorControllerServiceMock>("ValidateDuplicateElevatorRequest");
        var result = methodInfo.Invoke(_elevatorControllerServiceMock, new object[] { elevatorRequest }) as ElevatorRequestResult;

        if (isDuplicateRequest)
        {
            var duplicateElevatorRequestMessage = MessageService.FormatMessage(
                MessageService.DuplicateElevatorRequest,
                elevatorRequest.OriginFloor.ToString(),
                elevatorRequest.DestinationFloor.ToString(),
                elevatorRequest.Direction.ToString());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo(duplicateElevatorRequestMessage));
            Assert.That(_elevatorControllerServiceMock.CalledRequestQueueProducer, Is.False);
        } 
        else
        {
            Assert.That(result, Is.Null);
            Assert.That(_elevatorControllerServiceMock.CalledRequestQueueProducer, Is.True);
        }
    }

    /// <summary>
    /// RequestQueueProducer with expected result  
    /// </summary>
    [Test]
    public void RequestQueueProducer_ExpectedResult()
    {
        var requestQueueField = GetRequestQueueField();
        var requestQueue = new Queue<ElevatorRequest>();
        var elevatorRequest = new ElevatorRequest(_userInputRequest);
        requestQueue.Enqueue(elevatorRequest);

        requestQueueField.SetValue(_elevatorControllerServiceMock, requestQueue);

        _elevatorControllerServiceMock.CallBaseRequestQueueProducer = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorControllerServiceMock>("RequestQueueProducer");
        methodInfo.Invoke(_elevatorControllerServiceMock, new object[] { elevatorRequest });

        var requestQueueValue = GetRequestQueueValue() as Queue<ElevatorRequest>;
        Assert.That(requestQueueValue, Is.Not.Null);
        Assert.That(requestQueueValue.First(), Is.EqualTo(elevatorRequest));

        var queuedRequestMessage = elevatorRequest.ToQueuedRequestString();

        Assert.That(_elevatorControllerServiceMock.CalledUpdateRequestStatus, Is.True);
        Assert.That(
            _elevatorControllerServiceMock.CalledUpdateRequestStatusMessage?.Contains(queuedRequestMessage), Is.True);
    }

    /// <summary>
    /// RequestQueueConsumer with expected result  
    /// </summary>
    [Test]
    public void RequestQueueConsumer_ExpectedResult()
    {
        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorControllerServiceMock>("RequestElevatorAssignment");
        methodInfo.Invoke(_elevatorControllerServiceMock, null);

        Thread.Sleep(200);
        _elevatorControllerServiceMock.Exit();

        Assert.That(_elevatorControllerServiceMock.CalledRequestElevatorAssignment, Is.True);
    }

    /// <summary>
    /// RequestElevatorAssignment with expected result  
    /// </summary>
    /// <param name="isPending"></param>
    /// <param name="acceptPending"></param>
    /// <param name="isComplete"></param>
    /// <param name="hasQueuedRequest"></param>
    [Test]
    [TestCase(true, false, false)]
    [TestCase(true, true, false)]
    [TestCase(false, false, false)]
    [TestCase(false, true, false)]
    [TestCase(false, false, true)]
    [TestCase(false, false, true, false)]
    public void RequestElevatorAssignment_ExpectedResult(bool isPending, bool acceptPending, bool isComplete,
        bool hasQueuedRequest = true)
    {
        var newlyAcceptedElevatorId   = 1;
        var alreadyAcceptedElevatorId = 2;

        var completedRequestIdsField = GetCompletedRequestIdsField();
        var requestQueueField = GetRequestQueueField();

        var elevatorRequest = new ElevatorRequest(_userInputRequest);
        elevatorRequest.ElevatorId = isPending ? null : alreadyAcceptedElevatorId;

        var requestQueue = new Queue<ElevatorRequest>();

        if (hasQueuedRequest)
            requestQueue.Enqueue(elevatorRequest);

        requestQueueField.SetValue(_elevatorControllerServiceMock, requestQueue);

        if (isComplete)
        {
            completedRequestIdsField.SetValue(_elevatorControllerServiceMock, new HashSet<Guid> { elevatorRequest.Id });
            var completedRequestIdsValue = GetCompletedRequestIdsValue() as HashSet<Guid>;
            Assert.That(completedRequestIdsValue, Is.Not.Empty);
        }            

        _elevatorControllerServiceMock.RequestElevatorReturnValue = acceptPending ? newlyAcceptedElevatorId : null;
        _elevatorControllerServiceMock.CallBaseRequestElevatorAssignment = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorControllerServiceMock>("RequestElevatorAssignment");
        methodInfo.Invoke(_elevatorControllerServiceMock, null);

        if (!hasQueuedRequest)
        {
            Assert.That(_elevatorControllerServiceMock.CalledRequestElevator, Is.False);
            return;
        }

        var requestQueueValue = GetRequestQueueValue() as Queue<ElevatorRequest>;

        if (isComplete)
        {
            Assert.That(requestQueueValue, Is.Not.Null.And.Empty);

            var completedRequestIdsValue = GetCompletedRequestIdsValue() as HashSet<Guid>;
            Assert.That(completedRequestIdsValue, Is.Empty);
        }
        else
        {
            Assert.That(requestQueueValue, Is.Not.Null.And.Not.Empty);
            var processedRequest = requestQueueValue.First();

            if (isPending)
            {
                Assert.That(_elevatorControllerServiceMock.CalledRequestElevator, Is.True);

                if (acceptPending)
                    Assert.That(processedRequest.ElevatorId, Is.Not.Null.And.EqualTo(newlyAcceptedElevatorId));
                else
                    Assert.That(processedRequest.ElevatorId, Is.Null);
            }
            else
            {
                Assert.That(_elevatorControllerServiceMock.CalledRequestElevator, Is.False);
                Assert.That(processedRequest.ElevatorId, Is.Not.Null.And.EqualTo(alreadyAcceptedElevatorId));
            }
        }
    }

    /// <summary>
    /// RequestElevatorAssignment with expected exception message  
    /// </summary>
    [Test]
    public void RequestElevatorAssignment_ExpectedExceptionMessage()
    {
        var requestQueueField = GetRequestQueueField();
        requestQueueField.SetValue(_elevatorControllerServiceMock, null);

        _elevatorControllerServiceMock.CallBaseRequestElevatorAssignment = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorControllerServiceMock>("RequestElevatorAssignment");
        methodInfo.Invoke(_elevatorControllerServiceMock, null);

        Assert.That(_elevatorControllerServiceMock.CalledUpdateRequestStatus, Is.True);
        Assert.That(
            _elevatorControllerServiceMock.CalledUpdateRequestStatusMessage?.Contains(OtisSimConstants.ErrorProcessingRequest), Is.True);
    }

    /// <summary>
    /// RequestElevator with expected result 
    /// </summary>
    /// <param name="canAcceptCount"></param>
    /// <param name="prioritisedElevatorId"></param>
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(3)]
    [TestCase(3, 1)]
    [TestCase(3, 2)]
    [TestCase(3, 3)]
    [TestCase(2, 1)]
    [TestCase(2, 2)]
    public void RequestElevator_ExpectedResult(int canAcceptCount, int? prioritisedElevatorId = null)
    {
        var elevatorRequest = new ElevatorRequest(_userInputRequest);
        var nearestFloorNumber = int.Parse(_userInputRequest.OriginFloorInput!.ToString()!) + 1;

        for (var i = 0; i < canAcceptCount; i++)
        {
            var elevator = (_elevators[i] as ElevatorModelMock)!;

            elevator!.CanAcceptRequestReturnValue = true;
            elevator!.AcceptRequestReturnValue    = true;

            if (prioritisedElevatorId == null)
                elevator.CurrentFloor = nearestFloorNumber;
            else
            {
                elevator.CurrentFloor = elevator.Id == prioritisedElevatorId
                    ? nearestFloorNumber
                    : nearestFloorNumber + 2;
            }
        }

        var elevatorsField = GetElevatorsField();
        elevatorsField.SetValue(_elevatorControllerServiceMock, _elevators);

        _elevatorControllerServiceMock.CallBaseRequestElevator = true;

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorControllerServiceMock>("RequestElevator");
        var result = methodInfo.Invoke(_elevatorControllerServiceMock, new object[] { elevatorRequest });

        var acceptedCount = 0;
        for (var i = 0; i < _elevators.Count; i++)
        {
            var elevator = (_elevators[i] as ElevatorModelMock)!;
            Assert.That(elevator!.CalledCanAcceptRequest, Is.True);

            if (elevator.CanAcceptRequestReturnValue)
                acceptedCount += 1;
        }

        Assert.That(acceptedCount, Is.AtLeast(canAcceptCount));

        if (canAcceptCount == 0)
            Assert.That(result, Is.Null);
        else
        {
            Assert.That(result, Is.Not.Null.And.GreaterThan(0));

            var assignedElevator = _elevators.FirstOrDefault(elevator =>
            {
                var mockElevator = (elevator as ElevatorModelMock)!;
                return mockElevator.Id == (int)result &&
                    mockElevator.CanAcceptRequestReturnValue == true &&
                    mockElevator.AcceptRequestReturnValue == true;
            });
            
            Assert.That(assignedElevator, Is.Not.Null);

            if (prioritisedElevatorId != null)
                Assert.That(assignedElevator!.Id, Is.EqualTo((int)prioritisedElevatorId));
        }
    }

    /// <summary>
    /// CompleteRequest with expected result 
    /// </summary>
    /// <param name="requestIdAlreadyAdded"></param>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CompleteRequest_ExpectedResult(bool requestIdAlreadyAdded)
    {
        var requestId = Guid.NewGuid();

        var completedRequestIdsField = GetCompletedRequestIdsField();
        var initialValue = requestIdAlreadyAdded
            ? new HashSet<Guid> { requestId }
            : new HashSet<Guid>();

        completedRequestIdsField.SetValue(_elevatorControllerServiceMock, initialValue);

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorControllerServiceMock>("CompleteRequest");
        var result = methodInfo.Invoke(_elevatorControllerServiceMock, new object[] { requestId });

        var completedRequestIdsValue = GetCompletedRequestIdsValue() as HashSet<Guid>;

        Assert.That(completedRequestIdsValue!.Any(id => id == requestId), Is.True);
    }

    /// <summary>
    /// RequeueRequest with expected result
    /// </summary>
    [Test]
    public void RequeueRequest_ExpectedResult()
    {
        var elevatorRequest = new ElevatorRequest(_userInputRequest);
        elevatorRequest.ElevatorId = 2;

        var requestQueueField = GetRequestQueueField();
        var requestQueue = new Queue<ElevatorRequest>();
        requestQueue.Enqueue(elevatorRequest);

        requestQueueField.SetValue(_elevatorControllerServiceMock, requestQueue);

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorControllerServiceMock>("RequeueRequest");
        methodInfo.Invoke(_elevatorControllerServiceMock, new object[] { elevatorRequest.Id });

        Assert.That(elevatorRequest.ElevatorId, Is.Null);
    }

    /// <summary>
    /// PrintRequestStatus with expected result
    /// </summary>
    [Test]
    public void PrintRequestStatus_ExpectedResult()
    {
        var testMessage = nameof(PrintRequestStatus_ExpectedResult);

        var methodInfo = GetNonPublicInstanceMethodNotNull<ElevatorControllerServiceMock>("PrintRequestStatus");
        methodInfo.Invoke(_elevatorControllerServiceMock, new object[] { testMessage });

        Assert.That(_elevatorControllerServiceMock.CalledUpdateRequestStatus, Is.True);
        Assert.That(_elevatorControllerServiceMock.CalledUpdateRequestStatusMessage!.Contains(testMessage), Is.True);
    }

    /// <summary>
    /// GetElevatorsField
    /// </summary>
    /// <returns></returns>
    private FieldInfo GetElevatorsField()
        => GetNonPublicInstanceFieldNotNull<ElevatorControllerServiceMock>("_elevators");

    /// <summary>
    /// GetElevatorRequestValidationValuesField
    /// </summary>
    /// <returns></returns>
    private FieldInfo GetElevatorRequestValidationValuesField()
        => GetNonPublicInstanceFieldNotNull<ElevatorControllerServiceMock>("_elevatorRequestValidationValues");

    /// <summary>
    /// GetRequestQueueField
    /// </summary>
    /// <returns></returns>
    private FieldInfo GetRequestQueueField()
        => GetNonPublicInstanceFieldNotNull<ElevatorControllerServiceMock>("_requestQueue");

    /// <summary>
    /// GetRequestQueueValue
    /// </summary>
    /// <returns></returns>
    private object? GetRequestQueueValue()
    {
        var requestQueueField = GetRequestQueueField();
        return ValidateFieldValue(requestQueueField, _elevatorControllerServiceMock);
    }

    /// <summary>
    /// GetCompletedRequestIdsField
    /// </summary>
    /// <returns></returns>
    private FieldInfo GetCompletedRequestIdsField()
        => GetNonPublicInstanceFieldNotNull<ElevatorControllerServiceMock>("_completedRequestIds");

    /// <summary>
    /// GetCompletedRequestIdsValue
    /// </summary>
    /// <returns></returns>
    private object? GetCompletedRequestIdsValue()
    {
        var completedRequestIdsField = GetCompletedRequestIdsField();
        return ValidateFieldValue(completedRequestIdsField, _elevatorControllerServiceMock);
    }

    /// <summary>
    /// GetCancellationTokenSourceField
    /// </summary>
    /// <returns></returns>
    private FieldInfo GetCancellationTokenSourceField()
        => GetNonPublicInstanceFieldNotNull<ElevatorControllerServiceMock>("_cancellationTokenSource");

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private object? GetCancellationTokenSourceValue()
    {
        var cancellationTokenSourceField = GetCancellationTokenSourceField();
        return ValidateFieldValue(cancellationTokenSourceField, _elevatorControllerServiceMock);
    }

    /// <summary>
    /// GetRequestConsumerTaskField
    /// </summary>
    /// <returns></returns>
    private FieldInfo GetRequestConsumerTaskField()
        => GetNonPublicInstanceFieldNotNull<ElevatorControllerServiceMock>("_requestConsumerTask");

    /// <summary>
    /// GetRequestConsumerTaskValue
    /// </summary>
    /// <returns></returns>
    private object? GetRequestConsumerTaskValue()
    {
        var requestConsumerTaskField = GetRequestConsumerTaskField();
        return ValidateFieldValue(requestConsumerTaskField, _elevatorControllerServiceMock);
    }    
}

using ElevatorSystem.Application.Implementation;
using ElevatorSystem.Domain.Interface;
using Moq;

namespace ElevatorServiceTest.Tests;

public class ElevatorServiceTest
{
    [Fact]
    public async Task RunElevators_ShouldCallRunAsyncOnAllElevators()
    {
        var cancellationToken = new CancellationToken();
        var mockElevators = new List<Mock<IElevator>>()
        {
            new Mock<IElevator>(),
            new Mock<IElevator>(),
            new Mock<IElevator>(),
            new Mock<IElevator>()
        };

        foreach (var mockElevator in mockElevators)
        {
            mockElevator.Setup(e => e.RunAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        }

        var elevators = mockElevators.Select(m => m.Object).ToList();
        var elevatorService = new ElevatorService(elevators);

        await elevatorService.RunElevators(cancellationToken);

        foreach (var mockElevator in mockElevators)
        {
            mockElevator.Verify(e => e.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Fact]
    public void AssignRequestToElevator_ShouldAssignIdleOrSameDirectionElevator()
    {
        var mockElevator1 = new Mock<IElevator>();
        mockElevator1.Setup(e => e.IsMoving).Returns(false);
        mockElevator1.Setup(e => e.Direction).Returns("idle");
        mockElevator1.Setup(e => e.CurrentFloor).Returns(3);
        mockElevator1.Setup(e => e.UpRequests).Returns(new List<int>());
        mockElevator1.Setup(e => e.DownRequests).Returns(new List<int>());

        var mockElevator2 = new Mock<IElevator>();
        mockElevator2.Setup(e => e.IsMoving).Returns(true);
        mockElevator2.Setup(e => e.Direction).Returns("up");
        mockElevator2.Setup(e => e.CurrentFloor).Returns(7);
        mockElevator2.Setup(e => e.UpRequests).Returns(new List<int> { 5 });
        mockElevator2.Setup(e => e.DownRequests).Returns(new List<int>());

        var elevators = new List<IElevator> { mockElevator1.Object, mockElevator2.Object };
        var elevatorService = new ElevatorService(elevators);

        int requestFloor = 4;
        string direction = "up";

        var assignedElevator = elevatorService.AssignRequestToElevator(requestFloor, direction);

        Assert.Equal(mockElevator1.Object, assignedElevator); 
    }

    [Fact]
    public void AssignRequestToElevator_ShouldPrioritizeFewestRequests()
    {
        var mockElevator1 = new Mock<IElevator>();
        mockElevator1.Setup(e => e.IsMoving).Returns(false);
        mockElevator1.Setup(e => e.Direction).Returns("idle");
        mockElevator1.Setup(e => e.CurrentFloor).Returns(3);
        mockElevator1.Setup(e => e.UpRequests).Returns(new List<int> { 2, 6 });
        mockElevator1.Setup(e => e.DownRequests).Returns(new List<int>());

        var mockElevator2 = new Mock<IElevator>();
        mockElevator2.Setup(e => e.IsMoving).Returns(false);
        mockElevator2.Setup(e => e.Direction).Returns("idle");
        mockElevator2.Setup(e => e.CurrentFloor).Returns(5);
        mockElevator2.Setup(e => e.UpRequests).Returns(new List<int>());
        mockElevator2.Setup(e => e.DownRequests).Returns(new List<int>());

        var elevators = new List<IElevator> { mockElevator1.Object, mockElevator2.Object };
        var elevatorService = new ElevatorService(elevators);

        int requestFloor = 4;
        string direction = "up";

        var assignedElevator = elevatorService.AssignRequestToElevator(requestFloor, direction);

        Assert.Equal(mockElevator2.Object, assignedElevator);
    }

    [Fact]
    public void AssignRequestToElevator_ShouldAssignClosestElevatorIfTied()
    {
        var mockElevator1 = new Mock<IElevator>();
        mockElevator1.Setup(e => e.IsMoving).Returns(false);
        mockElevator1.Setup(e => e.Direction).Returns("idle");
        mockElevator1.Setup(e => e.CurrentFloor).Returns(3);
        mockElevator1.Setup(e => e.UpRequests).Returns(new List<int>());
        mockElevator1.Setup(e => e.DownRequests).Returns(new List<int>());

        var mockElevator2 = new Mock<IElevator>();
        mockElevator2.Setup(e => e.IsMoving).Returns(false);
        mockElevator2.Setup(e => e.Direction).Returns("idle");
        mockElevator2.Setup(e => e.CurrentFloor).Returns(6);
        mockElevator2.Setup(e => e.UpRequests).Returns(new List<int>());
        mockElevator2.Setup(e => e.DownRequests).Returns(new List<int>());

        var elevators = new List<IElevator> { mockElevator1.Object, mockElevator2.Object };
        var elevatorService = new ElevatorService(elevators);

        int requestFloor = 4;
        string direction = "up";

        var assignedElevator = elevatorService.AssignRequestToElevator(requestFloor, direction);

        Assert.Equal(mockElevator1.Object, assignedElevator);
    }

    [Fact]
    public void AssignRequestToElevator_ShouldAssignClosestElevator_WhenNoIdleOrSameDirectionAvailable()
    {
        var mockElevator1 = new Mock<IElevator>();
        mockElevator1.Setup(e => e.IsMoving).Returns(true);
        mockElevator1.Setup(e => e.Direction).Returns("down");
        mockElevator1.Setup(e => e.CurrentFloor).Returns(2);
        mockElevator1.Setup(e => e.UpRequests).Returns(new List<int>());
        mockElevator1.Setup(e => e.DownRequests).Returns(new List<int>());

        var mockElevator2 = new Mock<IElevator>();
        mockElevator2.Setup(e => e.IsMoving).Returns(true);
        mockElevator2.Setup(e => e.Direction).Returns("down");
        mockElevator2.Setup(e => e.CurrentFloor).Returns(5);
        mockElevator2.Setup(e => e.UpRequests).Returns(new List<int>());
        mockElevator2.Setup(e => e.DownRequests).Returns(new List<int>());

        var elevators = new List<IElevator> { mockElevator1.Object, mockElevator2.Object };
        var elevatorService = new ElevatorService(elevators);

        int requestFloor = 4;
        string direction = "up";

        var assignedElevator = elevatorService.AssignRequestToElevator(requestFloor, direction);

        Assert.Equal(mockElevator2.Object, assignedElevator); 
    }

    [Fact]
    public async Task RunSimulation_ShouldGenerateRequestsAndAssignElevator()
    {
        var mockElevator = new Mock<IElevator>();
        mockElevator.Setup(e => e.AddRequest(It.IsAny<int>(), It.IsAny<string>()));

        var mockService = new Mock<ElevatorService> { CallBase = true };

        mockService
            .Setup(s => s.AssignRequestToElevator(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(mockElevator.Object);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(3000);

        try
        {
            await mockService.Object.RunSimulation(cts.Token);
        }
        catch (TaskCanceledException)
        {
        }

        mockService.Verify(s => s.AssignRequestToElevator(It.IsAny<int>(), It.IsAny<string>()), Times.AtLeastOnce);
        mockElevator.Verify(e => e.AddRequest(It.IsAny<int>(), It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunSimulation_ShouldExit_WhenCancelled()
    {
        var mockElevator = new Mock<IElevator>();
        mockElevator.Setup(e => e.AddRequest(It.IsAny<int>(), It.IsAny<string>()));

        var mockService = new Mock<ElevatorService> { CallBase = true };
        mockService
            .Setup(s => s.AssignRequestToElevator(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(mockElevator.Object);

        using var cts = new CancellationTokenSource();
        cts.Cancel(); 

        await mockService.Object.RunSimulation(cts.Token);

        mockService.Verify(s => s.AssignRequestToElevator(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        mockElevator.Verify(e => e.AddRequest(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }
}

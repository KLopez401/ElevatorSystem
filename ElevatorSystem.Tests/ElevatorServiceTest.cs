using ElevatorSystem.Application.Implementation;
using ElevatorSystem.Domain.Interface;
using ElevatorSystem.Domain.Model;
using Moq;

namespace ElevatorServiceTest.Tests;

public class ElevatorServiceTest
{
    [Fact]
    public void AssignRequestToElevator_Should_Assign_To_Nearest_Elevator()
    {
        var elevator1 = new Elevator(1) { CurrentFloor = 1 };
        var elevator2 = new Elevator(2) { CurrentFloor = 5 };
        var elevators = new List<IElevator> { elevator1, elevator2 };
        var system = new ElevatorService(elevators);

        var assignedElevator = system.AssignRequestToElevator(3);

        Assert.Equal(elevator1, assignedElevator);
    }

    [Fact]
    public async Task RunElevators_Should_Call_RunAsync_On_All_Elevators()
    {
        var mockElevator1 = new Mock<IElevator>();
        var mockElevator2 = new Mock<IElevator>();
        var mockElevator3 = new Mock<IElevator>();
        var mockElevator4 = new Mock<IElevator>();

        mockElevator1.Setup(e => e.RunAsync(It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);
        mockElevator2.Setup(e => e.RunAsync(It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);
        mockElevator3.Setup(e => e.RunAsync(It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);
        mockElevator4.Setup(e => e.RunAsync(It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

        var elevators = new List<IElevator>
            {
                mockElevator1.Object,
                mockElevator2.Object,
                mockElevator3.Object,
                mockElevator4.Object
            };

        var elevatorSystem = new ElevatorService(elevators);
        var cancellationToken = new CancellationTokenSource().Token;

        await elevatorSystem.RunElevators(cancellationToken);

        mockElevator1.Verify(e => e.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockElevator2.Verify(e => e.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockElevator3.Verify(e => e.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockElevator4.Verify(e => e.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

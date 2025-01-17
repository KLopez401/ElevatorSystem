using ElevatorSystem.Application.Implementation;
using ElevatorSystem.Domain.Interface;
using Moq;

namespace ElevatorServiceTest.Tests;

public class ElevatorServiceTest
{
    [Fact]
    public void AssignRequestToElevator_Should_Assign_To_Nearest_Elevator()
    {
        var mockElevator1 = new Mock<IElevator>();
        var mockElevator2 = new Mock<IElevator>();

        mockElevator1.Setup(e => e.CurrentFloor).Returns(1);
        mockElevator2.Setup(e => e.CurrentFloor).Returns(5);

        var elevators = new List<IElevator> { mockElevator1.Object, mockElevator2.Object };
        var system = new ElevatorService(elevators);

        var assignedElevator = system.AssignRequestToElevator(3, "up");

        Assert.Equal(mockElevator1.Object, assignedElevator);
    }

    [Fact]
    public void AssignRequestToElevator_Should_Prioritize_Idle_Elevator()
    {
        var mockElevator1 = new Mock<IElevator>();
        var mockElevator2 = new Mock<IElevator>();

        mockElevator1.Setup(e => e.CurrentFloor).Returns(3);
        mockElevator1.Setup(e => e.IsMoving).Returns(true);
        mockElevator2.Setup(e => e.CurrentFloor).Returns(3);
        mockElevator2.Setup(e => e.IsMoving).Returns(false);

        var elevators = new List<IElevator> { mockElevator1.Object, mockElevator2.Object };
        var system = new ElevatorService(elevators);

        var assignedElevator = system.AssignRequestToElevator(3, "up");

        Assert.Equal(mockElevator2.Object, assignedElevator);
    }
}

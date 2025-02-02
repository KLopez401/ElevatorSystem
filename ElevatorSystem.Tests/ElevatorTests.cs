using ElevatorSystem.Domain.Model;
using Moq;

namespace ElevatorTests.Tests;

public class ElevatorTests
{
    [Fact]
    public void AddRequest_ShouldAddUpRequest_WhenDirectionIsUp()
    {
        var elevator = new Elevator();
        int floor = 5;

        elevator.AddRequest(floor, "up");

        Assert.Contains(floor, elevator.UpRequests);
        Assert.DoesNotContain(floor, elevator.DownRequests);
    }
    [Fact]
    public void AddRequest_ShouldAddDownRequest_WhenDirectionIsDown()
    {
        var elevator = new Elevator();
        int floor = 7;

        elevator.AddRequest(floor, "down");

        Assert.Contains(floor, elevator.DownRequests);
        Assert.DoesNotContain(floor, elevator.UpRequests);
    }
    [Fact]
    public void AddRequest_ShouldSortDownRequestsDescending()
    {
        var elevator = new Elevator();
        var floors = new List<int> { 3, 9, 6 };

        foreach (var floor in floors)
            elevator.AddRequest(floor, "down");

        Assert.Equal(new List<int> { 9, 6, 3 }, elevator.DownRequests);
    }

    [Fact]
    public void AddRequest_Should_Add_Up_Requests_Correctly()
    {
        var elevator = new Elevator(1);

        elevator.AddRequest(5, "up");

        Assert.Contains(5, elevator.UpRequests);
    }

    [Fact]
    public void AddRequest_Should_Add_Down_Requests_Correctly()
    {
        var elevator = new Elevator(1);

        elevator.AddRequest(2, "down");

        Assert.Contains(2, elevator.DownRequests);
    }

    [Fact]
    public void Elevator_Should_Not_Add_Duplicate_Requests()
    {
        var elevator = new Elevator(1);

        elevator.AddRequest(5, "up");
        elevator.AddRequest(5, "up");

        Assert.Single(elevator.UpRequests);
    }

    [Fact]
    public async Task Elevator_Should_Not_Move_When_Idle()
    {
        var elevator = new Elevator(1);
        var cancellationToken = new CancellationTokenSource().Token;

        var task = elevator.RunAsync(cancellationToken);
        await Task.Delay(5000);

        Assert.Equal("idle", elevator.Direction);
        Assert.False(elevator.IsMoving);
    }

    [Fact]
    public async Task ProcessRequests_ShouldProcessEachRequestAndRemoveIt()
    {
        var mockElevator = new Mock<Elevator> { CallBase = true };
        var requests = new List<int> { 3, 7, 5 };
        var cancellationToken = new CancellationToken();

        mockElevator
            .Setup(e => e.MoveToFloor(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await mockElevator.Object.ProcessRequests(requests, cancellationToken);

        Assert.Empty(requests);
        mockElevator.Verify(e => e.MoveToFloor(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task ProcessRequests_ShouldProcessRequestsInOrder()
    {
        var elevator = new Elevator();
        var processedFloors = new List<int>();
        var requests = new List<int> { 2, 5, 8 };
        var cancellationToken = new CancellationToken();

        var mockElevator = new Mock<Elevator> { CallBase = true };
        mockElevator
            .Setup(e => e.MoveToFloor(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Callback<int, CancellationToken>((floor, _) => processedFloors.Add(floor))
            .Returns(Task.CompletedTask);

        await mockElevator.Object.ProcessRequests(requests, cancellationToken);

        Assert.Equal(new List<int> { 2, 5, 8 }, processedFloors);
    }

    [Fact]
    public async Task ProcessRequests_ShouldExitImmediately_IfNoRequests()
    {
        var mockElevator = new Mock<Elevator> { CallBase = true };
        var requests = new List<int>();
        var cancellationToken = new CancellationToken();

        await mockElevator.Object.ProcessRequests(requests, cancellationToken);

        mockElevator.Verify(e => e.MoveToFloor(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
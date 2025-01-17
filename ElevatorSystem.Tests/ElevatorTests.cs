using ElevatorSystem.Domain.Model;

namespace ElevatorTests.Tests;

public class ElevatorTests
{
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

        elevator.AddRequest(3, "down");

        Assert.Contains(3, elevator.DownRequests);
    }

    [Fact]
    public async Task RunAsync_Should_Process_Up_Requests_First()
    {
        var elevator = new Elevator(1);
        elevator.AddRequest(3, "up");
        elevator.AddRequest(5, "up");
        var cancellationToken = new CancellationTokenSource().Token;

        var task = elevator.RunAsync(cancellationToken);
        await Task.Delay(35000);

        Assert.Empty(elevator.UpRequests);
        Assert.Equal("idle", elevator.Direction);
    }

    [Fact]
    public async Task RunAsync_Should_Process_Down_Requests_After_Up()
    {
        var elevator = new Elevator(1);
        elevator.AddRequest(3, "up");
        elevator.AddRequest(2, "down");

        var cancellationToken = new CancellationTokenSource().Token;
        var task = elevator.RunAsync(cancellationToken);
        await Task.Delay(45000);

        Assert.Empty(elevator.UpRequests);
        Assert.Empty(elevator.DownRequests);
        Assert.Equal("idle", elevator.Direction);
    }
}
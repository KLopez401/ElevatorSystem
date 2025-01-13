using ElevatorSystem.Domain.Model;

namespace ElevatorTests.Tests;

public class ElevatorTests
{
    [Fact]
    public void AddRequest_Should_Add_Floor_To_Requests()
    {
        var elevator = new Elevator(1);

        elevator.AddRequest(5);

        Assert.Contains(5, elevator.Requests);
    }

    [Fact]
    public void AddRequest_Should_Not_Add_Duplicate_Floors()
    {
        var elevator = new Elevator(1);

        elevator.AddRequest(5);
        elevator.AddRequest(5);

        Assert.Single(elevator.Requests);
    }

    [Fact]
    public void UpdateDirection_Should_Set_Direction_To_Up_When_Target_Is_Higher()
    {
        var elevator = new Elevator(1);
        elevator.AddRequest(5);

        elevator.UpdateDirection();

        Assert.Equal("up", elevator.Direction);
    }
}
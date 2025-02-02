using ElevatorSystem.Domain.Interface;

namespace ElevatorSystem.Application.Implementation;

public class ElevatorService : IElevatorService
{

    private readonly List<IElevator> _elevators;
    private readonly Random _random = new();
    public ElevatorService()
    {
        
    }
    public ElevatorService(List<IElevator> elevators)
    {
        _elevators = elevators;
    }
    public async Task RunElevators(CancellationToken cancellationToken)
    {
        var elevatorTasks = _elevators.Select(e => e.RunAsync(cancellationToken)).ToList();
        await Task.WhenAll(elevatorTasks);
    }

    public virtual IElevator AssignRequestToElevator(int floor, string direction)
    {
        return _elevators
                   .Where(e => !e.IsMoving || e.Direction == direction) 
                   .OrderBy(e => e.UpRequests.Count + e.DownRequests.Count) 
                   .ThenBy(e => Math.Abs(e.CurrentFloor - floor)) 
                   .FirstOrDefault() ?? _elevators.OrderBy(e => Math.Abs(e.CurrentFloor - floor)).First();
    }
    public async Task RunSimulation(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            int floor = _random.Next(1, 11);
            string direction = _random.Next(0, 2) == 0 ? "up" : "down";

            var elevator = AssignRequestToElevator(floor, direction);
            elevator.AddRequest(floor, direction);

            Console.WriteLine($"[REQUEST] New {direction} request at floor {floor}.");

            await Task.Delay(_random.Next(1000, 3000), cancellationToken);
        }
    }
}

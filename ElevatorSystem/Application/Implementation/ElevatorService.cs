using ElevatorSystem.Domain.Interface;

namespace ElevatorSystem.Application.Implementation;

public class ElevatorService : IElevatorService
{

    private readonly List<IElevator> _elevators;

    public ElevatorService(List<IElevator> elevators)
    {
        _elevators = elevators;
    }
    public async Task HandleRequests(Func<Task<(int floor, string direction)>> requestGenerator, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var (floor, direction) = await requestGenerator();

                var elevator = AssignRequestToElevator(floor, direction);
                elevator.AddRequest(floor, direction);

                await Task.Delay(500, cancellationToken); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public async Task RunElevators(CancellationToken cancellationToken)
    {
        var elevatorTasks = _elevators.Select(e => e.RunAsync(cancellationToken)).ToList();
        await Task.WhenAll(elevatorTasks);
    }

    public IElevator AssignRequestToElevator(int floor, string direction)
    {
        return _elevators
            .OrderBy(e => Math.Abs(e.CurrentFloor - floor))
            .ThenBy(e => e.IsMoving ? 1 : 0)
            .First();
    }
}

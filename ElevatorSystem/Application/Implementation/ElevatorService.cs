using ElevatorSystem.Domain.Interface;

namespace ElevatorSystem.Application.Implementation;

public class ElevatorService : IElevatorService
{

    private readonly List<IElevator> _elevators;

    public ElevatorService(List<IElevator> elevators)
    {
        _elevators = elevators;
    }
    public async Task HandleUserInput(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Enter floor requests separated by commas (e.g., 3,5,7):");
                var input = Console.ReadLine();
                var floorRequests = ParseFloorRequests(input);

                if (floorRequests.Count > 0)
                {
                    foreach (var floor in floorRequests)
                    {
                        var elevator = AssignRequestToElevator(floor);
                        elevator.AddRequest(floor);
                    }
                }

                await Task.Delay(500);
            }
            catch
            {
                Console.WriteLine("Error processing input. Please try again.");
            }
        }
    }

    public async Task RunElevators(CancellationToken cancellationToken)
    {
        var elevatorTasks = _elevators.Select(e => e.RunAsync(cancellationToken)).ToList();
        await Task.WhenAll(elevatorTasks);
    }

    private List<int> ParseFloorRequests(string? input)
    {
        try
        {
            return input?.Split(',')
                .Select(f => int.Parse(f.Trim()))
                .Where(f => f >= 1 && f <= 10)
                .ToList() ?? new List<int>();
        }
        catch
        {
            Console.WriteLine("Invalid input. Please enter valid floor numbers separated by commas.");
            return new List<int>();
        }
    }

    private IElevator AssignRequestToElevator(int floor)
    {
        return _elevators
            .OrderBy(e => Math.Abs(e.CurrentFloor - floor))
            .ThenBy(e => e.IsMoving ? 1 : 0)
            .First();
    }
}

using ElevatorSystem.Application.Implementation;
using ElevatorSystem.Domain.Interface;
using ElevatorSystem.Domain.Model;

namespace ElevatorSystem.Presentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
            Console.WriteLine("\nShutting down...");
        };

        var elevators = new List<IElevator>
            {
                new Elevator(1),
                new Elevator(2),
                new Elevator(3),
                new Elevator(4)
            };

        var system = new ElevatorService(elevators);

        var requestTask = system.HandleRequests(GetUserRequest, cts.Token);
        var elevatorTask = system.RunElevators(cts.Token);

        await Task.WhenAny(requestTask, elevatorTask);
    }

    private static async Task<(int floor, string direction)> GetUserRequest()
    {
        Console.WriteLine("Enter floor and direction (e.g., '3 up' or '5 down'):");
        var input = Console.ReadLine()?.Split(' ');
        if (input != null && input.Length == 2 && int.TryParse(input[0], out var floor))
        {
            return (floor, input[1]);
        }

        Console.WriteLine("Invalid input. Please try again.");
        return await GetUserRequest();
    }
}

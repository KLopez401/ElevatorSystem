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

        var inputTask = system.HandleUserInput(cts.Token);
        var elevatorTask = system.RunElevators(cts.Token);

        await Task.WhenAny(inputTask, elevatorTask);
    }
}

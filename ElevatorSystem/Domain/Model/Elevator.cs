using ElevatorSystem.Domain.Interface;

namespace ElevatorSystem.Domain.Model;

public class Elevator : IElevator
{
    public int Id { get; }
    public int CurrentFloor { get; private set; }
    public List<int> UpRequests { get; private set; } = new();
    public List<int> DownRequests { get; private set; } = new();
    public string Direction { get; private set; } = "idle";
    public bool IsMoving { get; private set; }

    public Elevator(int id)
    {
        Id = id;
        CurrentFloor = 1; 
    }

    public void AddRequest(int floor, string direction)
    {
        if (direction == "up" && !UpRequests.Contains(floor))
        {
            UpRequests.Add(floor);
            UpRequests.Sort();
            Console.WriteLine($"Elevator {Id}: New person going up at floor {floor}.");
        }
        else if (direction == "down" && !DownRequests.Contains(floor))
        {
            DownRequests.Add(floor);
            DownRequests.Sort((a, b) => b.CompareTo(a));
            Console.WriteLine($"Elevator {Id}: New person going down at floor {floor}.");
        }

        UpdateDirection();
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (UpRequests.Count == 0 && DownRequests.Count == 0)
            {
                Direction = "idle";
                await Task.Delay(1000, cancellationToken); 
                continue;
            }

            IsMoving = true;

            if (Direction == "up")
            {
                await ProcessRequests(UpRequests, cancellationToken);

                Direction = DownRequests.Count > 0 ? "down" : "idle";
            }
            else if (Direction == "down")
            {
                await ProcessRequests(DownRequests, cancellationToken);

                Direction = UpRequests.Count > 0 ? "up" : "idle";
            }
        }
    }

    private async Task ProcessRequests(List<int> requests, CancellationToken cancellationToken)
    {
        while (requests.Count > 0)
        {
            int targetFloor = requests.First();
            await MoveToFloor(targetFloor, cancellationToken);
            requests.Remove(targetFloor);
            Console.WriteLine($"Elevator {Id}: Person got off at floor {targetFloor}.");
        }
    }

    private async Task MoveToFloor(int targetFloor, CancellationToken cancellationToken)
    {
        while (CurrentFloor != targetFloor && !cancellationToken.IsCancellationRequested)
        {
            CurrentFloor += CurrentFloor < targetFloor ? 1 : -1;
            Console.WriteLine($"Elevator {Id}: Moving to floor {CurrentFloor}...");
            await Task.Delay(10000, cancellationToken); 
        }

        Console.WriteLine($"Elevator {Id}: Stopped at floor {CurrentFloor}.");
        await Task.Delay(10000, cancellationToken);
    }

    public void UpdateDirection()
    {
        if (Direction == "idle")
        {
            Direction = UpRequests.Count > 0 ? "up" : DownRequests.Count > 0 ? "down" : "idle";
        }
    }
}
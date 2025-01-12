using ElevatorSystem.Domain.Interface;

namespace ElevatorSystem.Domain.Model;

public class Elevator : IElevator
{
    public int Id { get; }
    public int CurrentFloor { get; private set; }
    public bool IsMoving { get; private set; }
    public string Direction { get; private set; } = "idle";
    public List<int> Requests { get; private set; } = new();

    public Elevator(int id)
    {
        Id = id;
        CurrentFloor = 1;
    }

    public void AddRequest(int floor)
    {
        if (!Requests.Contains(floor))
        {
            Requests.Add(floor);
            Requests.Sort();
            Console.WriteLine($"Elevator {Id}: Request added for floor {floor}.");
            UpdateDirection();
        }
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (Requests.Count == 0)
            {
                Direction = "idle";
                await Task.Delay(1000, cancellationToken);
                continue;
            }

            IsMoving = true;

            var targetFloors = Direction == "up"
                ? Requests.Where(floor => floor > CurrentFloor).ToList()
                : Requests.Where(floor => floor < CurrentFloor).OrderByDescending(floor => floor).ToList();

            if (targetFloors.Count == 0)
            {
                Direction = Direction == "up" ? "down" : "up";
                continue;
            }

            foreach (var targetFloor in targetFloors)
            {
                while (CurrentFloor != targetFloor && !cancellationToken.IsCancellationRequested)
                {
                    CurrentFloor += Direction == "up" ? 1 : -1;
                    Console.WriteLine($"Elevator {Id}: Moving to floor {CurrentFloor}...");
                    await Task.Delay(10000, cancellationToken);
                }

                Console.WriteLine($"Elevator {Id}: Stopped at floor {CurrentFloor}.");
                Requests.Remove(CurrentFloor); 
            }

            UpdateDirection();
        }
    }

    private void UpdateDirection()
    {
        if (Requests.Count == 0)
        {
            Direction = "idle";
            return;
        }

        if (Requests.Any(floor => floor > CurrentFloor))
        {
            Direction = "up";
        }
        else if (Requests.Any(floor => floor < CurrentFloor))
        {
            Direction = "down";
        }
    }
}
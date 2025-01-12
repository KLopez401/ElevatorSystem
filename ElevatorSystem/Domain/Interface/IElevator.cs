namespace ElevatorSystem.Domain.Interface;

public interface IElevator
{
    int Id { get; }
    int CurrentFloor { get; }
    bool IsMoving { get; }
    string Direction { get; }
    List<int> Requests { get; }

    void AddRequest(int floor);
    Task RunAsync(CancellationToken cancellationToken);
}

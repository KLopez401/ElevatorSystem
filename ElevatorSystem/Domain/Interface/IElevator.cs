namespace ElevatorSystem.Domain.Interface;

public interface IElevator
{
    int Id { get; }
    int CurrentFloor { get; }
    List<int> UpRequests { get; }
    List<int> DownRequests { get; }
    string Direction { get; }
    bool IsMoving { get; }

    void AddRequest(int floor, string direction);
    Task RunAsync(CancellationToken cancellationToken);
    Task ProcessRequests(List<int> requests, CancellationToken cancellationToken);
    Task MoveToFloor(int targetFloor, CancellationToken cancellationToken);
}

namespace ElevatorSystem.Domain.Interface;

public interface IElevatorService
{
    Task HandleRequests(Func<Task<(int floor, string direction)>> requestGenerator, CancellationToken cancellationToken);
    Task RunElevators(CancellationToken cancellationToken);
    IElevator AssignRequestToElevator(int floor, string direction);
}

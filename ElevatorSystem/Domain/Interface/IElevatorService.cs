namespace ElevatorSystem.Domain.Interface;

public interface IElevatorService
{
    Task RunElevators(CancellationToken cancellationToken);
    IElevator AssignRequestToElevator(int floor, string direction);
    Task RunSimulation(CancellationToken cancellationToken);
}

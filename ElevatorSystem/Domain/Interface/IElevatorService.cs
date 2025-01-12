namespace ElevatorSystem.Domain.Interface;

public interface IElevatorService
{
    Task HandleUserInput(CancellationToken cancellationToken);
    Task RunElevators(CancellationToken cancellationToken);
}

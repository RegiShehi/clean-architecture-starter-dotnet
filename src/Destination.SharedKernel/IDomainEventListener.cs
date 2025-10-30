namespace Destination.SharedKernel;

public interface IDomainEventListener<in T> where T : IDomainEvent
{
    Task Handle(T domainEvent, CancellationToken cancellationToken);
}

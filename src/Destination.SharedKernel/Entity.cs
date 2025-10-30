namespace Destination.SharedKernel;

public abstract class Entity
{
    protected Entity()
    {
    }

    protected Entity(Guid id) => Id = id;

    public Guid Id { get; init; }

    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => [.. _domainEvents];

    public void ClearDomainEvents() => _domainEvents.Clear();

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Loqora.Domain.Common;

public abstract class Entity : ISoftDeletable
{
    public Guid Id { get; }

    public bool IsDeleted { get; private set; }

    public DateTimeOffset? DeletedOnUtc { get; private set; }

    public string? DeletedBy { get; private set; }

    private readonly List<DomainEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Entity() { }

    protected Entity(Guid id)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
    }

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Delete(string? deletedBy = null, DateTimeOffset? deletedOnUtc = null)
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedOnUtc = deletedOnUtc ?? DateTimeOffset.UtcNow;

    }

    public void Restore()
    {
        if (!IsDeleted)
        {
            return;
        }

        IsDeleted = false;
        DeletedBy = null;
        DeletedOnUtc = null;
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange.Domain.Common;

public abstract class BaseEntity
{
    Guid _Id;
    public virtual Guid Id
    {
        get
        {
            return _Id;
        }
        set
        {
            _Id = value;
        }
    }

    private readonly List<BaseEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

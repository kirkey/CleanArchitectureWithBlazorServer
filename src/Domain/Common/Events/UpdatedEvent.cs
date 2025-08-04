using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Common.Events;

public class UpdatedEvent<T>(T entity) : DomainEvent
    where T : IEntity
{
    public T Entity { get; } = entity;
}
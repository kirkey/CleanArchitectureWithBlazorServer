// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations.Schema;
using CleanArchitecture.Blazor.Domain.Common.Events;

namespace CleanArchitecture.Blazor.Domain.Common.Entities;

public abstract class BaseUlidEntity : IEntity<string>
{
    private readonly List<DomainEvent> _domainEvents = new();

    [NotMapped] public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public virtual string Id { get; set; } = Ulid.NewUlid().ToString();

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
}

public abstract class BaseUlidAuditableEntity : BaseUlidEntity, IAuditableEntity
{
    public virtual DateTime? Created { get; set; }

    public virtual string? CreatedBy { get; set; }

    public virtual DateTime? LastModified { get; set; }

    public virtual string? LastModifiedBy { get; set; }
}

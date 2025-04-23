// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Blazor.Domain.Common.Entities;

public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
{
    public virtual string? Description { get; set; }
    public virtual string? Notes { get; set; }
    
    public virtual DateTime? CreatedOn { get; set; }

    public virtual string? CreatedBy { get; set; }
    public virtual string? CreatedUser { get; set; }

    public virtual DateTime? LastModifiedOn { get; set; }

    public virtual string? LastModifiedBy { get; set; }
    public virtual string? LastModifiedUser { get; set; }
}

public interface IAuditableEntity
{
    DateTime? CreatedOn { get; set; }

    string? CreatedBy { get; set; }
    string? CreatedUser { get; set; }

   DateTime? LastModifiedOn { get; set; }

    string? LastModifiedBy { get; set; }
    string? LastModifiedUser { get; set; }
}
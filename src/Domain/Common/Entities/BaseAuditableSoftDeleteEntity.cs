﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Common.Entities;

public abstract class BaseAuditableSoftDeleteEntity : BaseAuditableEntity, ISoftDelete
{
    public DateTime? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeletedUser { get; set; }
}
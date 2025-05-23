﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Common.Enums;
using CleanArchitecture.Blazor.Domain.Identity;


namespace CleanArchitecture.Blazor.Domain.Entities;

public sealed class Document : BaseAuditableEntity, IMayHaveTenant, IAuditTrial
{
    public string? Title { get; set; }
    public JobStatus Status { get; set; } = default!;
    public string? Content { get; set; }
    public bool IsPublic { get; set; }
    public string? Url { get; set; }
    public DocumentType DocumentType { get; set; }
    public Tenant? Tenant { get; set; }
    public string? TenantId { get; set; }

    public ApplicationUser? CreatedByUser { get; set; }
    public ApplicationUser? LastModifiedByUser { get; set; }
}

public enum DocumentType
{
    Document,
    Excel,
    Image,
    Pdf,
    Others
}
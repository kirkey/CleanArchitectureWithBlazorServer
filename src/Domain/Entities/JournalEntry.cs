// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class JournalEntry : BaseUlidAuditableEntity
{
    public DateTime Date { get; set; } = DateTime.Now;
    public string? ReferenceNumber { get; set; }
    public string? Description { get; set; }
    public string? Source { get; set; }
    public bool IsPosted { get; set; } = false;
    public List<GeneralLedger>? LedgerLines { get; set; }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class Invoice : BaseUlidAuditableEntity
{
    public string? CustomerID { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Open;
    public List<InvoiceItem>? Items { get; set; }
}

public enum InvoiceStatus
{
    Open,
    Paid,
    Overdue
}

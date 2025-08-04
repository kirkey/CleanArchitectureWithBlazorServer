// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class Bill : BaseUlidAuditableEntity
{
    public string? VendorID { get; set; }
    public DateTime BillDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public BillStatus Status { get; set; } = BillStatus.Open;
    public List<BillItem>? Items { get; set; }
}

public enum BillStatus
{
    Open,
    Paid,
    Overdue
}

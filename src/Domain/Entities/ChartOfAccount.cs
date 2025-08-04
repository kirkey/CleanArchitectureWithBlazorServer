// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class ChartOfAccount : BaseUlidAuditableEntity
{
    public AccountType AccountType { get; set; }
    public string? SubAccountOf { get; set; }
    public bool IsActive { get; set; } = true;
}

public enum AccountType
{
    Asset,
    Liability,
    Equity,
    Revenue,
    Expense
}

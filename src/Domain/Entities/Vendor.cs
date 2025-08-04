// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class Vendor : BaseUlidAuditableEntity
{
    public string? VendorName { get; set; }
    public string? BillingAddress { get; set; }
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Terms { get; set; }
}

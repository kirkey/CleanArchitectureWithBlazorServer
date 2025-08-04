// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IApplicationDbContext: IAsyncDisposable
{
    DbSet<SystemLog> SystemLogs { get; set; }
    DbSet<AuditTrail> AuditTrails { get; set; }
    DbSet<Document> Documents { get; set; }
    DbSet<PicklistSet> PicklistSets { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Tenant> Tenants { get; set; }
    DbSet<Contact> Contacts { get; set; }
    DbSet<LoginAudit> LoginAudits { get; set; }
    DbSet<UserLoginRiskSummary> UserLoginRiskSummaries { get; set; }
    
    // Accounting Domains
    DbSet<ChartOfAccount> ChartOfAccounts { get; set; }
    DbSet<JournalEntry> JournalEntries { get; set; }
    DbSet<GeneralLedger> GeneralLedgers { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Vendor> Vendors { get; set; }
    DbSet<Invoice> Invoices { get; set; }
    DbSet<InvoiceItem> InvoiceItems { get; set; }
    DbSet<Payment> Payments { get; set; }
    DbSet<Bill> Bills { get; set; }
    DbSet<BillItem> BillItems { get; set; }
    
    ChangeTracker ChangeTracker { get; }

    DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

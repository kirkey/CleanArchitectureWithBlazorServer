// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class GeneralLedgerConfiguration : IEntityTypeConfiguration<GeneralLedger>
{
    public void Configure(EntityTypeBuilder<GeneralLedger> builder)
    {
        builder.Property(t => t.Id).HasMaxLength(36);
        builder.Property(t => t.EntryId).HasMaxLength(36);
        builder.Property(t => t.AccountId).HasMaxLength(36);
        builder.Property(t => t.Memo).HasMaxLength(500);
        builder.Property(t => t.Debit).HasPrecision(18, 2);
        builder.Property(t => t.Credit).HasPrecision(18, 2);
        builder.HasIndex(x => x.EntryId);
        builder.HasIndex(x => x.AccountId);
        builder.Ignore(e => e.DomainEvents);
    }
}

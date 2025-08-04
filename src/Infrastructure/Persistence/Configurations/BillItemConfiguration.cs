// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class BillItemConfiguration : IEntityTypeConfiguration<BillItem>
{
    public void Configure(EntityTypeBuilder<BillItem> builder)
    {
        builder.Property(t => t.Id).HasMaxLength(36);
        builder.Property(t => t.BillId).HasMaxLength(36);
        builder.Property(t => t.Name).HasMaxLength(256);
        builder.Property(t => t.UnitPrice).HasPrecision(18, 2);
        builder.Property(t => t.LineTotal).HasPrecision(18, 2);
        builder.HasIndex(x => x.BillId);
        builder.Ignore(e => e.DomainEvents);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.Property(t => t.Id).HasMaxLength(36);
        builder.Property(t => t.VendorId).HasMaxLength(36);
        builder.Property(t => t.TotalAmount).HasPrecision(18, 2);
        builder.HasIndex(x => x.VendorId);
        builder.HasIndex(x => x.BillDate);
        builder.HasIndex(x => x.Status);
        builder.Ignore(e => e.DomainEvents);

        // Configure enum
        builder.Property(e => e.Status)
            .HasConversion<string>();

        // Configure relationship with BillItems
        builder.HasMany(b => b.Items)
            .WithOne()
            .HasForeignKey(bi => bi.BillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

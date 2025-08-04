// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.Property(t => t.Id).HasMaxLength(36);
        builder.Property(t => t.CustomerID).HasMaxLength(36);
        builder.Property(t => t.TotalAmount).HasPrecision(18, 2);
        builder.HasIndex(x => x.CustomerID);
        builder.HasIndex(x => x.InvoiceDate);
        builder.HasIndex(x => x.Status);
        builder.Ignore(e => e.DomainEvents);

        // Configure enum
        builder.Property(e => e.Status)
            .HasConversion<string>();

        // Configure relationship with InvoiceItems
        builder.HasMany(i => i.Items)
            .WithOne()
            .HasForeignKey(ii => ii.InvoiceID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

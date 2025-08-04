// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(t => t.Id).HasMaxLength(36);
        builder.Property(t => t.CustomerId).HasMaxLength(36);
        builder.Property(t => t.Amount).HasPrecision(18, 2);
        builder.Property(t => t.ReferenceNumber).HasMaxLength(100);
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.PaymentDate);
        builder.HasIndex(x => x.ReferenceNumber);
        builder.Ignore(e => e.DomainEvents);

        // Configure enum
        builder.Property(e => e.PaymentMethod)
            .HasConversion<string>();
    }
}

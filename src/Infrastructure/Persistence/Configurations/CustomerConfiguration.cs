// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(t => t.Id)
            .HasMaxLength(36);  // ULID length

        builder.Property(t => t.Name)
            .HasMaxLength(256);

        builder.Property(t => t.BillingAddress)
            .HasMaxLength(500);

        builder.Property(t => t.PhoneNumber)
            .HasMaxLength(100);

        builder.Property(t => t.Email)
            .HasMaxLength(256);

        builder.Property(t => t.Terms)
            .HasMaxLength(100);

        builder.Ignore(e => e.DomainEvents);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.Property(t => t.Id).HasMaxLength(36);
        builder.Property(t => t.VendorName).HasMaxLength(256).IsRequired();
        builder.Property(t => t.BillingAddress).HasMaxLength(500);
        builder.Property(t => t.ContactPerson).HasMaxLength(100);
        builder.Property(t => t.Email).HasMaxLength(254);
        builder.Property(t => t.Terms).HasMaxLength(100);
        builder.HasIndex(x => x.VendorName);
        builder.HasIndex(x => x.Email);
        builder.Ignore(e => e.DomainEvents);
    }
}

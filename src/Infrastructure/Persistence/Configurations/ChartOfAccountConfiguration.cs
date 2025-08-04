// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class ChartOfAccountConfiguration : IEntityTypeConfiguration<ChartOfAccount>
{
    public void Configure(EntityTypeBuilder<ChartOfAccount> builder)
    {
        builder.Property(t => t.Id).HasMaxLength(36);
        builder.Property(t => t.Name).HasMaxLength(256).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.SubAccountOf).HasMaxLength(36);
        builder.HasIndex(x => x.Name);
        builder.Ignore(e => e.DomainEvents);

        // Configure enum
        builder.Property(e => e.AccountType)
            .HasConversion<string>();
    }
}

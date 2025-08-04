// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.Property(t => t.Id).HasMaxLength(36);
        builder.Property(t => t.ReferenceNumber).HasMaxLength(100);
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.Source).HasMaxLength(100);
        builder.HasIndex(x => x.ReferenceNumber);
        builder.HasIndex(x => x.Date);
        builder.Ignore(e => e.DomainEvents);
    }
}

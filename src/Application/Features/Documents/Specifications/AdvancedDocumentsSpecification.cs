﻿namespace CleanArchitecture.Blazor.Application.Features.Documents.Specifications;
#nullable disable warnings
public class AdvancedDocumentsSpecification : Specification<Document>
{
    public AdvancedDocumentsSpecification(AdvancedDocumentsFilter filter)
    {
        var today = DateTime.UtcNow;
        var todayrange = today.GetDateRange("TODAY", filter.CurrentUser.LocalTimeOffset);
        var last30daysrange = today.GetDateRange("LAST_30_DAYS", filter.CurrentUser.LocalTimeOffset);
        Query.Where(p =>
                    (p.CreatedBy == filter.CurrentUser.UserId && p.IsPublic == false) ||
                    (p.IsPublic == true && p.TenantId == filter.CurrentUser.TenantId),
                filter.ListView == DocumentListView.All)
            .Where(p =>
                    p.CreatedBy == filter.CurrentUser.UserId && p.TenantId == filter.CurrentUser.TenantId,
                filter.ListView == DocumentListView.My)
            .Where(x => x.CreatedOn >= todayrange.Start && x.CreatedOn < todayrange.End.AddDays(1),
                filter.ListView == DocumentListView.TODAY)
            .Where(x => x.CreatedOn >= last30daysrange.Start, filter.ListView == DocumentListView.LAST_30_DAYS)
            .Where(
                x => x.Title.Contains(filter.Keyword) || x.Description.Contains(filter.Keyword) ||
                     x.Content.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}
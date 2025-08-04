namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Specifications;
#nullable disable warnings
public class PicklistSetAdvancedSpecification : Specification<PicklistSet>
{
    public PicklistSetAdvancedSpecification(PicklistSetAdvancedFilter filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange(PickListView.Today.ToString(), filter.CurrentUser.LocalTimeOffset);
        var last30daysrange = today.GetDateRange(PickListView.Last30Days.ToString(), filter.CurrentUser.LocalTimeOffset);

        Query.Where(p => p.Name == filter.Picklist, filter.Picklist is not null)
             .Where(q => q.CreatedBy == filter.CurrentUser.UserId, filter.ListView == PickListView.My && filter.CurrentUser is not null)
             .Where(x => x.Created >= todayrange.Start && x.Created < todayrange.End.AddDays(1), filter.ListView == PickListView.Today)
             .Where(x => x.Created >= last30daysrange.Start, filter.ListView == PickListView.Last30Days)
             .Where(
                x => x.Description.Contains(filter.Keyword) || x.Text.Contains(filter.Keyword) ||
                     x.Value.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.ChartOfAccounts.Specifications;

public enum ChartOfAccountListView
{
    [Description("All")]
    All,
    [Description("My")]
    My,
    [Description("Created Today")]
    TODAY,
    [Description("Created within the last 30 days")]
    LAST_30_DAYS
}

public class ChartOfAccountAdvancedFilter : PaginationFilter
{
    public ChartOfAccountListView ListView { get; set; } = ChartOfAccountListView.All;
    public UserProfile? CurrentUser { get; set; }
    public string? Keyword { get; set; }
}

public class ChartOfAccountAdvancedSpecification : Specification<ChartOfAccount>
{
    public ChartOfAccountAdvancedSpecification(ChartOfAccountAdvancedFilter filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange(ChartOfAccountListView.TODAY.ToString(), filter.CurrentUser?.LocalTimeOffset ?? TimeSpan.FromMinutes(0));
        var last30daysrange = today.GetDateRange(ChartOfAccountListView.LAST_30_DAYS.ToString(), filter.CurrentUser?.LocalTimeOffset ?? TimeSpan.FromMinutes(0));

        Query.Where(q => q.Name != null)
             .Where(filter.Keyword, !string.IsNullOrEmpty(filter.Keyword))
             .Where(q => q.CreatedBy == filter.CurrentUser.UserId, filter.ListView == ChartOfAccountListView.My && filter.CurrentUser is not null)
             .Where(x => x.Created >= todayrange.Start && x.Created < todayrange.End.AddDays(1), filter.ListView == ChartOfAccountListView.TODAY)
             .Where(x => x.Created >= last30daysrange.Start, filter.ListView == ChartOfAccountListView.LAST_30_DAYS);
    }
}
namespace CleanArchitecture.Blazor.Application.Features.SystemLogs.Specifications;

public enum SystemLogListView
{
    [Description("All")] 
    All,
    [Description("Created Today")] 
    Today,
    [Description("View of the last 30 days")]
    Last30Days
}

public class SystemLogAdvancedFilter : PaginationFilter
{
    public UserProfile? CurrentUser { get; set; }
    public LogLevel? Level { get; set; }
    public SystemLogListView ListView { get; set; } = SystemLogListView.Last30Days;
}
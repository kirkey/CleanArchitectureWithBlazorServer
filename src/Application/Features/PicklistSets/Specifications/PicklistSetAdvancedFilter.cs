namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Specifications;

public enum PickListView
{
    [Description("All")]
    All,
    [Description("My")]
    My,
    [Description("Created Today")]
    Today,
    [Description("Created within the last 30 days")]
    Last30Days
}
public class PicklistSetAdvancedFilter : PaginationFilter
{
    public PickListView ListView { get; set; } = PickListView.All;
    public UserProfile? CurrentUser { get; set; }
    public Picklist? Picklist { get; set; }
}
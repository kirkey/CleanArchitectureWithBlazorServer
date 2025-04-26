using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public class OrgChart(IJSRuntime jsRuntime)
{
    public async Task<ValueTask> Create(IList<OrgItem> data)
    {
        var jsmodule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/orgchart.js")
            .ConfigureAwait(false);
        return jsmodule.InvokeVoidAsync("createOrgChart", data);
    }
}
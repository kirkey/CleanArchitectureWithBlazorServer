using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public class HistoryGo(IJSRuntime jsRuntime)
{
    public async Task<ValueTask> GoBack(int value = -1)
    {
        var jsmodule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/historygo.js")
            .ConfigureAwait(false);
        return jsmodule.InvokeVoidAsync(JsInteropConstants.HistoryGo, value);
    }
}
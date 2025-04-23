using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public class OpenSeadragon(IJSRuntime jsRuntime)
{
    public async Task<ValueTask> Open(string url)
    {
        var target = "openseadragon";
        var jsmodule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/openseadragon.js").ConfigureAwait(false);
        return jsmodule.InvokeVoidAsync(JsInteropConstants.ShowOpenSeadragon, target, url);
    }
}
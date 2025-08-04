using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public class InputClear(IJSRuntime jsRuntime)
{
    public async Task<ValueTask> Clear(string targetId)
    {
        var jsmodule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/clearinput.js").ConfigureAwait(false);
        return jsmodule.InvokeVoidAsync(JSInteropConstants.ClearInput, targetId);
    }
}
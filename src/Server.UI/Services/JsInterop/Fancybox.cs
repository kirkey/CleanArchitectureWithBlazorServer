using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public sealed class Fancybox(IJSRuntime jsRuntime)
{
    public async Task<ValueTask> Preview(string defaultUrl, IEnumerable<string> images)
    {
        var jsmodule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/fancybox.js").ConfigureAwait(false);
        return jsmodule.InvokeVoidAsync(JsInteropConstants.PreviewImage, defaultUrl,
            images);
    }
}
using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public sealed class Swiper(IJSRuntime jsRuntime)
{
    public async Task<ValueTask> Initialize(string elment, bool reverse = false)
    {
        var jsmodule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/carousel.js")
            .ConfigureAwait(false);
        return jsmodule.InvokeVoidAsync("initializeSwiper", elment, reverse);
    }
}
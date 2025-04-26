using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Blazor.Server.UI.Hubs;

public class ServerHubWrapper(IHubContext<ServerHub, ISignalRHub> hubContext) : IApplicationHubWrapper
{
    public async Task JobStarted(int id, string message)
    {
        await hubContext.Clients.All.Start(id, message).ConfigureAwait(false);
    }

    public async Task JobCompleted(int id, string message)
    {
        await hubContext.Clients.All.Completed(id, message).ConfigureAwait(false);
    }
}
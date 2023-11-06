using Microsoft.AspNetCore.SignalR.Client;

namespace TaskManagerApp.Services;

public interface ISignalRClientService
{
    HubConnection? GetHubConnection();
}

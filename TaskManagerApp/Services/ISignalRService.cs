namespace TaskManagerApp.Services;

public interface ISignalRService
{
    Task SendMessage<T>(string topic, T message);
}

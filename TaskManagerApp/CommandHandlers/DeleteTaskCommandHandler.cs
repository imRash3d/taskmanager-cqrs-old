using Newtonsoft.Json;
using TaskManagerApp.Commands;
using TaskManagerApp.Dto;
using TaskManagerApp.Services;
using TaskManagerApp.Utils;

namespace TaskManagerApp.CommandHandlers;

public class DeleteTaskCommandHandler : IHandler<DeleteTaskCommand>
{
    private readonly ITaskService _service;
    private readonly ILogger<DeleteTaskCommandHandler> _logger;

    public DeleteTaskCommandHandler(
        ITaskService service,
        ILogger<DeleteTaskCommandHandler> logger
    )
    {
        _service = service;
        _logger = logger;
    }

    public HandlerResponse Handle(DeleteTaskCommand signal)
    {
        return HandleAsync(signal).Result;
    }

    public async Task<HandlerResponse> HandleAsync(DeleteTaskCommand signal)
    {
        _logger.LogInformation("Enter {} with payload: {}",
            GetType().Name, JsonConvert.SerializeObject(signal));
        try
        {
            await _service.Delete(signal.Id);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }

        return new HandlerResponse(null);
    }
}

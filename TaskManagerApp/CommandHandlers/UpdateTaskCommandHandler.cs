using Newtonsoft.Json;
using TaskManagerApp.Commands;
using TaskManagerApp.Dto;
using TaskManagerApp.Services;
using TaskManagerApp.Utils;

namespace TaskManagerApp.CommandHandlers;

public class UpdateTaskCommandHandler : IHandler<UpdateTaskCommand>
{
    private readonly ITaskService _service;
    private readonly ILogger<UpdateTaskCommandHandler> _logger;

    public UpdateTaskCommandHandler(ITaskService service, ILogger<UpdateTaskCommandHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public HandlerResponse Handle(UpdateTaskCommand signal)
    {
        return HandleAsync(signal).Result;
    }

    public async Task<HandlerResponse> HandleAsync(UpdateTaskCommand signal)
    {
        _logger.LogInformation("Enter {} with payload: {}",
            GetType().Name, JsonConvert.SerializeObject(signal));
        try
        {
            var result = await _service.UpdateAsync(signal);
            return new HandlerResponse(new List<object> { result });
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }
    }
}

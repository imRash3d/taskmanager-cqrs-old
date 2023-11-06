using Newtonsoft.Json;
using TaskManagerApp.Commands;
using TaskManagerApp.Dto;
using TaskManagerApp.Services;
using TaskManagerApp.Utils;

namespace TaskManagerApp.CommandHandlers;

public class AddTaskCommandHandler : IHandler<AddTaskCommand>
{
    private readonly ITaskService _service;
    private readonly ILogger<AddTaskCommandHandler> _logger;

    public AddTaskCommandHandler(
        ITaskService service,
        ILogger<AddTaskCommandHandler> logger
    )
    {
        _service = service;
        _logger = logger;
    }

    public HandlerResponse Handle(AddTaskCommand signal)
    {
        return HandleAsync(signal).Result;
    }

    public async Task<HandlerResponse> HandleAsync(AddTaskCommand signal)
    {
        _logger.LogInformation("Enter {} with payload: {}",
            GetType().Name, JsonConvert.SerializeObject(signal));
        try
        {
            var result = await _service.AddAsync(signal);
            return new HandlerResponse(new List<object> { result });
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }
    }
}

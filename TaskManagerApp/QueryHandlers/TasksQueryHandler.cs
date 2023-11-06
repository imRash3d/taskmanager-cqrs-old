using Newtonsoft.Json;
using TaskManagerApp.Dto;
using TaskManagerApp.Queries;
using TaskManagerApp.Services;
using TaskManagerApp.Utils;

namespace TaskManagerApp.QueryHandlers;

public class TasksQueryHandler : IHandler<TasksQuery>
{
    private readonly ITaskService _service;
    private readonly ILogger<TasksQueryHandler> _logger;

    public TasksQueryHandler(ITaskService service, ILogger<TasksQueryHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public HandlerResponse Handle(TasksQuery signal)
    {
        return HandleAsync(signal).Result;
    }

    public async Task<HandlerResponse> HandleAsync(TasksQuery query)
    {
        _logger.LogInformation("Enter {TaskByIdHandlerName} with payload: {SerializeObject}",
            GetType().Name, JsonConvert.SerializeObject(query));
        try
        {
            var results = await _service.GetAsync(query);
            return new HandlerResponse(results);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }
    }
}

using System.Text;
using Microsoft.AspNetCore.SignalR.Client;
using TaskManagerApp.Commands;
using TaskManagerApp.Dto;
using TaskManagerApp.Models;
using TaskManagerApp.Queries;
using TaskManagerApp.Repository;
using TaskManagerApp.Utils;

namespace TaskManagerApp.Services;

public class TaskService : ITaskService
{
    private readonly ILogger<TaskService> _logger;
    private readonly IRepository _repository;
    private readonly IRabbitMqService _mqService;
    private readonly ISignalRService _signalR;
    private readonly HubConnection? _hubConnection;

    private const string TaskExchangeName = "TaskExchange";
    private const string TaskRouteAdd = "Task.add";
    private const string TaskRouteUpdate = "Task.update";
    private const string TaskRouteDelete = "Task.delete";

    public TaskService(
        ILogger<TaskService> logger,
        IRabbitMqService mqService,
        ISignalRService signalR,
        ISignalRClientService signalRClient,
        IRepository repository
    )
    {
        _logger = logger;
        _mqService = mqService;
        _signalR = signalR;
        _hubConnection = signalRClient.GetHubConnection();
        _repository = repository;
        AddConsumer();
        if (_hubConnection != null)
        {
            ConnectSocket();
        }
    }

    public async Task<List<TaskResponse>> GetAsync(TasksQuery query)
    {
        if (query.Id.HasValue)
        {
            return new List<TaskResponse>()
            {
                GetAsync(query.Id.Value).Result
            };
        }

        var Tasks = await _repository.Get<TaskManager>(null, query);

        var userIds = Tasks.Select(Task => Task.CreateUserGuid)
            .Concat(Tasks.Select(Task => Task.AssignedUserGuid))
            .Distinct()
            .ToList();

        var users = await GetUsers(userIds);

        return Tasks
            .Select(Task => GetDto(Task, users).Result)
            .ToList();
    }

    public async Task<TaskResponse> GetAsync(Guid id)
    {
        var entity = await _repository.Get<TaskManager>(id);
        return GetDto(entity, null).Result;
    }

    public async Task<TaskResponse> AddAsync(AddTaskCommand dto)
    {
        try
        {
            _logger.LogInformation("Task add call {}", dto);
            var entity = dto.GetTask();
            await _repository.Insert(entity);
            _mqService.SendMessage(TaskExchangeName, TaskRouteAdd, entity);
            await _signalR.SendMessage(TaskRouteAdd, entity);
            return GetDto(entity, null).Result;
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found at Task add. {}", e.Message);
            throw;
        }
    }

    public async Task<TaskResponse> UpdateAsync(UpdateTaskCommand dto)
    {
        try
        {
            _logger.LogInformation("Task update call {}", dto);
            var entity = await _repository.Get<TaskManager>(dto.Id);
            dto.UpdateTask(entity);

            await _repository.Update(entity);
            _mqService.SendMessage(TaskExchangeName, TaskRouteUpdate, entity);
            await _signalR.SendMessage(TaskRouteUpdate, entity);

            return GetDto(entity, null).Result;
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found at Task update. {}", e.Message);
            throw;
        }
    }

    public async Task Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("Task delete call {}", id);
            await _repository.Delete<TaskManager>(id);
            _mqService.SendMessage(TaskExchangeName, TaskRouteDelete, id);
            await _signalR.SendMessage(TaskRouteDelete, id);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found at Task delete. {}", e.Message);
            throw;
        }
    }

    private async Task<TaskResponse> GetDto(TaskManager entity, List<User>? users)
    {

    
        if (users == null)
        {
            var userIds = new List<Guid>
            {
                entity.CreateUserGuid,
                entity.AssignedUserGuid
            };
            users = await GetUsers(userIds.Distinct().ToList());
        }

        var dto = TaskResponse.GetDto(entity);
        dto.CreatedUser = UserGetDto.GetUserDto(users.FirstOrDefault(u => u.Id == entity.CreateUserGuid));
        dto.AssignedUser = UserGetDto.GetUserDto(users?.FirstOrDefault(u => u.Id == entity.AssignedUserGuid));

        return dto;
    }

    private Task<List<User>> GetUsers(ICollection<Guid> userIds)
    {
        try
        {
            return _repository
                .Get<User>(u => userIds.Contains(u.Id), new Paging());
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Trace, "Exception found {}", e.Message);
            throw new Exception(e.Message);
        }
    }

    private void AddConsumer()
    {
        var addConsumer = _mqService.AddConsumer(TaskExchangeName, TaskRouteAdd);
        if (addConsumer != null)
            addConsumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body;
                    var decodedString = Encoding.UTF8.GetString(body.ToArray());
                    // var Task = JsonConvert.DeserializeObject<Task>(body.ToString());
                    _logger.Log(LogLevel.Information, "Received message for routing key  {}, data {}", TaskRouteAdd,
                        decodedString);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Trace, "Exception found {}", e.Message);
                }
            };

        var updateConsumer = _mqService.AddConsumer(TaskExchangeName, TaskRouteUpdate);
        if (updateConsumer != null)
            updateConsumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body;
                    var decodedString = Encoding.UTF8.GetString(body.ToArray());
                    // var Task = JsonConvert.DeserializeObject<Task>(decodedString);
                    _logger.Log(LogLevel.Information, "Received message for routing key  {}, data {}", TaskRouteUpdate,
                        decodedString);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Trace, "Exception found {}", e.Message);
                }
            };

        var deleteConsumer = _mqService.AddConsumer(TaskExchangeName, TaskRouteDelete);
        if (deleteConsumer != null)
            deleteConsumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body;
                    var decodedString = Encoding.UTF8.GetString(body.ToArray());
                    _logger.Log(LogLevel.Information, "Received message for routing key  {}, data {}", TaskRouteDelete,
                        decodedString);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Trace, "Exception found {}", e.Message);
                }
            };
    }

    private void ConnectSocket()
    {
        if (_hubConnection == null) return;
        _hubConnection.On<string>(TaskRouteAdd,
            (message) =>
            {
                _logger.Log(LogLevel.Information, "socket message {} on topic {}", message, TaskRouteAdd);
            });

        _hubConnection.On<string>(TaskRouteUpdate,
            (message) =>
            {
                _logger.Log(LogLevel.Information, "socket message {} on topic {}", message, TaskRouteUpdate);
            });

        _hubConnection.On<string>(TaskRouteDelete,
            (message) =>
            {
                _logger.Log(LogLevel.Information, "socket message {} on topic {}", message, TaskRouteDelete);
            });
    }
}

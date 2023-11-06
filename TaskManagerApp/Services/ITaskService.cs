using TaskManagerApp.Commands;
using TaskManagerApp.Dto;
using TaskManagerApp.Queries;

namespace TaskManagerApp.Services;

public interface ITaskService
{
    public Task<List<TaskResponse>> GetAsync(TasksQuery query);
    public Task<TaskResponse> GetAsync(Guid id);
    public Task<TaskResponse> AddAsync(AddTaskCommand dto);
    public Task<TaskResponse> UpdateAsync(UpdateTaskCommand dto);
    public Task Delete(Guid id);
}

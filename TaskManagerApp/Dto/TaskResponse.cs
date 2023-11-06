using System.ComponentModel.DataAnnotations;
using TaskManagerApp.Models;

namespace TaskManagerApp.Dto;

public class TaskResponse : ResponseBase
{
    [Required] public string Name { get; set; }=String.Empty;
    [Required] public string Description { get; set; } = String.Empty;
    public bool Complete { get; set; }
    public TaskStatus Status { get; set; }
    public UserGetDto? CreatedUser { get; set; }
    public UserGetDto? AssignedUser { get; set; }

    public static TaskResponse GetDto(TaskManager Task)
    {
        var dto = new TaskResponse
        {
            Id = Task.Id,
            Name = Task.Name,
            Description = Task.Description,
            Complete = Task.Complete,
            Status = (TaskStatus)Task.Status,
            CreateDate = Task.CreateDate,
            UpdateDate = Task.UpdateDate
        };
        return dto;
    }
}

using System.ComponentModel.DataAnnotations;
using TaskManagerApp.Models;

namespace TaskManagerApp.Commands;

public class AddTaskCommand
{
    [Required] public string Name { get; set; } = String.Empty;
    [Required] public string Description { get; set; } = String.Empty;
    [Required] public Guid CreatedUserGuid { get; set; }
    [Required] public Guid AssignedUserGuid { get; set; }

    public TaskManager GetTask()
    {
        return new TaskManager
        {
            Id = Guid.NewGuid(),
            Name = Name,
            Description = Description,
            CreateDate = DateTime.Now,
            CreateUserGuid = CreatedUserGuid,
            AssignedUserGuid = AssignedUserGuid,
            Complete = false,
            Status = TaskManagerStatus.Pending
        };
    }
}

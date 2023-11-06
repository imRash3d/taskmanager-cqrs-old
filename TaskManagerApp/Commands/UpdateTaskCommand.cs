using System.ComponentModel.DataAnnotations;
using TaskManagerApp.Models;

namespace TaskManagerApp.Commands;

public class UpdateTaskCommand
{
    [Required] public Guid Id { get; set; }
    [Required] public string Name { get; set; } = String.Empty;
    [Required] public string Description { get; set; } = String.Empty;
    public bool Complete { get; set; }
    public TaskManagerStatus Status { get; set; }

    public void UpdateTask(TaskManager Task)
    {
        Task.Name = Name;
        Task.Description = Description;
        Task.Complete = Complete;
        Task.Status = Status;
        Task.UpdateDate = DateTime.Now;
    }
}

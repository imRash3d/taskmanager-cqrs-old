namespace TaskManagerApp.Models;

public class TaskManager : EntityBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Complete { get; set; }
    public Guid CreateUserGuid { get; set; }
    public Guid AssignedUserGuid { get; set; }
    public TaskManagerStatus Status { get; set; }
}

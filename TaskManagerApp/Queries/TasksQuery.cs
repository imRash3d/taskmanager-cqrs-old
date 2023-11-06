using TaskManagerApp.Utils;

namespace TaskManagerApp.Queries;

public class TasksQuery : Paging
{
    public Guid? Id { get; set; }
}

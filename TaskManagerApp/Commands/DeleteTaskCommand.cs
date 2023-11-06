using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Commands;

public class DeleteTaskCommand
{
    [Required] public Guid Id { get; set; }
}

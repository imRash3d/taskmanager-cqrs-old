using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApp.Commands;
using TaskManagerApp.Dto;
using TaskManagerApp.Queries;
using TaskManagerApp.Utils;

namespace TaskManagerApp.Controllers;

[Authorize]
[Route("api/Tasks")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly IHandlerService _handler;

    public TasksController(IHandlerService handler)
    {
        _handler = handler;
    }

    [HttpPost("get")]
    public async Task<HandlerResponse> Get(TasksQuery query)
    {
        return await _handler.HandleAsync(query);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddTaskCommand command)
    {
        var result = await _handler.HandleAsync(command);
        var data = ControllerUtil.GetData<TaskResponse>(result);

        return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
    }

    [HttpPost("update")]
    public async Task<TaskResponse> Update(UpdateTaskCommand command)
    {
        var result = await _handler.HandleAsync(command);

        return ControllerUtil.GetData<TaskResponse>(result);
    }

    [HttpPost("delete")]
    public IActionResult Delete(DeleteTaskCommand command)
    {
        _handler.Handle(command);
        return NoContent();
    }
}

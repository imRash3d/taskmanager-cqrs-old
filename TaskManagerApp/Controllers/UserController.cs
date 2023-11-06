using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApp.Dto;
using TaskManagerApp.Models;
using TaskManagerApp.Services;
using TaskManagerApp.Utils;

namespace TaskManagerApp.Controllers;

[AllowAnonymous]
[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        ILogger<UsersController> logger,
        IUserService userService)
    {
        _logger = logger;
        _service = userService;
    }

    [HttpGet("")]
    public async Task<List<UserGetDto?>> Get(Paging page)
    {
        //get users
        var users = await _service.GetAsync(page);

        //convert to dto
        var response = users.Select(u => Task.FromResult(UserGetDto.GetUserDto(u)));

        
        return (await Task.WhenAll(response)).ToList();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add(UserCreateDto user)
    {
        try
        {
            await _service.CreateAsync(user);
            _logger.Log(LogLevel.Information, "User added {}", user);
        }
        catch (Exception ex)
        {
            _logger.LogError("User added failed ", ex.Message);
        }

        return Created("user", null);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        UserUpdateDto dto
    )
    {
        try
        {
            dto.Id = id;
            var user = await _service.UpdateAsync(dto);
            _logger.Log(LogLevel.Information, "User updated {}", user);
        }
        catch (Exception ex)
        {
            _logger.LogError( "User updated failed ", ex.Message);
        }

        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<UserGetDto?> GetById(
        [FromRoute] Guid id
    )
    {
        return await _service.GetById(id);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id
    )
    {
        try
        {
            await _service.DeleteById(id);
            _logger.Log(LogLevel.Information, "User delete {}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError("User deleted failed ", ex.Message);
        }

        return Ok();
    }
}

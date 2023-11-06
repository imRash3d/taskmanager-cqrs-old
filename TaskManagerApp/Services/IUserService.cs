using TaskManagerApp.Dto;
using TaskManagerApp.Models;
using TaskManagerApp.Utils;

namespace TaskManagerApp.Services;

public interface IUserService
{
    public Task CreateAsync(UserCreateDto dto);
    public Task<List<User>> GetAsync(Paging paging);
    public Task<UserGetDto?> UpdateAsync(UserUpdateDto dto);
    public Task<UserGetDto> GetById(Guid id);
    public Task DeleteById(Guid id);
}

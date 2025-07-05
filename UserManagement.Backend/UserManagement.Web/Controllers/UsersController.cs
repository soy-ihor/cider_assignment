using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public Task<PaginatedResponseDto<UserDto>> GetUsers(
        [FromQuery] string? name,
        [FromQuery] string? email,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        => userService.GetUsersAsync(name, email, page, pageSize);

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await userService.GetUserByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public Task<UserDto> CreateUser([FromBody] CreateUserDto dto)
        => userService.CreateUserAsync(dto);

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        var user = await userService.UpdateUserAsync(id, dto);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
        => await userService.DeleteUserAsync(id) ? NoContent() : NotFound();

    [HttpPost("generate")]
    public Task<List<UserDto>> GenerateUsers()
        => userService.ImportUsersFromJsonPlaceholderAsync();

    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderUsers([FromBody] ReorderUsersDto dto)
        => await userService.ReorderUsersAsync(dto.UserIds) ? NoContent() : BadRequest();
} 
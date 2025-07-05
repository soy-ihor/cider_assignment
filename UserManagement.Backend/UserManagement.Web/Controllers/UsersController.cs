using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(
    IUserService userService,
    ILogger<UsersController> logger
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<UserDto>>> GetUsers(
        [FromQuery] string? name,
        [FromQuery] string? email,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await userService.GetUsersAsync(name, email, page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting users");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        try
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user with ID {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userService.CreateUserAsync(createUserDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userService.UpdateUserAsync(id, updateUserDto);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user with ID {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        try
        {
            var success = await userService.DeleteUserAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting user with ID {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("generate")]
    public async Task<ActionResult<List<UserDto>>> GenerateUsers()
    {
        try
        {
            var importedUsers = await userService.ImportUsersFromJsonPlaceholderAsync();
            return Ok(importedUsers);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating users from JSONPlaceholder");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("reorder")]
    public async Task<ActionResult> ReorderUsers([FromBody] ReorderUsersDto reorderDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await userService.ReorderUsersAsync(reorderDto.UserIds);
            if (!success)
                return BadRequest("Invalid user IDs provided");

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reordering users");
            return StatusCode(500, "Internal server error");
        }
    }
} 
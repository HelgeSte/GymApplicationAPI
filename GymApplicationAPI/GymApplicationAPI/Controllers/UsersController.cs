using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymApplicationAPI.Data;
using GymApplicationAPI.DTOs;
using BCrypt.Net;

namespace GymApplicationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly GymDbContext _context;

    public UsersController(GymDbContext context)
    {
        _context = context;
    }

    // GET /api/users?page=0&pageSize=30
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserDto>>> GetUsers(int page = 0, int pageSize = 30)
    {
        var query = _context.Users.OrderBy(u => u.Username);

        var totalCount = await query.CountAsync();

        var users = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                DateOfBirth = u.DateOfBirth,
                IsDisabled = u.IsDisabled,
                Role = u.Role
            })
            .ToListAsync();

        return Ok(new PagedResult<UserDto>
        {
            Items = users,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    // GET /api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        return Ok(new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            IsDisabled = user.IsDisabled,
            Role = user.Role
        });
    }

    // PUT /api/users/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, UpdateUserRequest request)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != id))
            return BadRequest("Email already in use.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.DateOfBirth = request.DateOfBirth;

        await _context.SaveChangesAsync();

        return Ok(new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            IsDisabled = user.IsDisabled,
            Role = user.Role
        });
    }

    // PATCH /api/users/{id}/password
    [HttpPatch("{id}/password")]
    public async Task<IActionResult> UpdatePassword(int id, UpdatePasswordRequest request)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return BadRequest("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PATCH /api/users/{id}/disable
    [HttpPatch("{id}/disable")]
    public async Task<IActionResult> DisableUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        user.IsDisabled = !user.IsDisabled;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

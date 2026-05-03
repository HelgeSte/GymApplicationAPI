using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymApplicationAPI.Data;
using GymApplicationAPI.DTOs;
using GymApplicationAPI.Models;

namespace GymApplicationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly GymDbContext _context;

    public SessionsController(GymDbContext context)
    {
        _context = context;
    }

    // GET /api/sessions?year=2026&month=5
    [HttpGet]
    public async Task<ActionResult<List<SessionDto>>> GetSessions(int? year, int? month)
    {
        var query = _context.Sessions
            .Include(s => s.Bookings)
            .AsQueryable();

        if (year.HasValue && month.HasValue)
        {
            query = query.Where(s => s.Date.Year == year.Value && s.Date.Month == month.Value);
        }

        var sessions = await query
            .OrderBy(s => s.Date)
            .ThenBy(s => s.Time)
            .Select(s => new SessionDto
            {
                Id = s.Id,
                Name = s.Name,
                Location = s.Location,
                Date = s.Date,
                Time = s.Time,
                TotalSpots = s.TotalSpots,
                AvailableSpots = s.TotalSpots - s.Bookings.Count,
                Color = s.Color
            })
            .ToListAsync();

        return Ok(sessions);
    }

    // GET /api/sessions/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<SessionDto>> GetSession(int id)
    {
        var session = await _context.Sessions
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null)
            return NotFound();

        return Ok(new SessionDto
        {
            Id = session.Id,
            Name = session.Name,
            Location = session.Location,
            Date = session.Date,
            Time = session.Time,
            TotalSpots = session.TotalSpots,
            AvailableSpots = session.TotalSpots - session.Bookings.Count,
            Color = session.Color
        });
    }

    // POST /api/sessions
    [HttpPost]
    public async Task<ActionResult<SessionDto>> CreateSession(CreateSessionRequest request)
    {
        var session = new Session
        {
            Name = request.Name,
            Location = request.Location,
            Date = request.Date,
            Time = request.Time,
            TotalSpots = request.TotalSpots,
            Color = request.Color
        };

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSession), new { id = session.Id }, new SessionDto
        {
            Id = session.Id,
            Name = session.Name,
            Location = session.Location,
            Date = session.Date,
            Time = session.Time,
            TotalSpots = session.TotalSpots,
            AvailableSpots = session.TotalSpots,
            Color = session.Color
        });
    }

    // PUT /api/sessions/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<SessionDto>> UpdateSession(int id, UpdateSessionRequest request)
    {
        var session = await _context.Sessions
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null)
            return NotFound();

        session.Name = request.Name;
        session.Location = request.Location;
        session.Date = request.Date;
        session.Time = request.Time;
        session.TotalSpots = request.TotalSpots;
        session.Color = request.Color;

        await _context.SaveChangesAsync();

        return Ok(new SessionDto
        {
            Id = session.Id,
            Name = session.Name,
            Location = session.Location,
            Date = session.Date,
            Time = session.Time,
            TotalSpots = session.TotalSpots,
            AvailableSpots = session.TotalSpots - session.Bookings.Count,
            Color = session.Color
        });
    }

    // DELETE /api/sessions/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSession(int id)
    {
        var session = await _context.Sessions.FindAsync(id);

        if (session == null)
            return NotFound();

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
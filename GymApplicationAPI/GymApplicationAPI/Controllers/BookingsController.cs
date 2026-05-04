using GymApplicationAPI.Data;
using GymApplicationAPI.DTOs;
using GymApplicationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymApplicationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly GymDbContext _context;

    public BookingsController(GymDbContext context)
    {
        _context = context;
    }

    // GET /api/bookings/session/{sessionId} - list all users booked for a session
    [HttpGet("session/{sessionId}")]
    public async Task<ActionResult<List<BookingDto>>> GetBookingsBySession(int sessionId)
    {
        var session = await _context.Sessions.FindAsync(sessionId);
        if (session == null)
            return NotFound("Session not found.");

        var bookings = await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Session)
            .Where(b => b.SessionId == sessionId)
            .OrderBy(b => b.BookedAt)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                UserId = b.UserId,
                Username = b.User.Username,
                FirstName = b.User.FirstName,
                LastName = b.User.LastName,
                SessionId = b.SessionId,
                SessionName = b.Session.Name,
                SessionDate = b.Session.Date,
                SessionTime = b.Session.Time,
                BookedAt = b.BookedAt
            })
            .ToListAsync();

        return Ok(bookings);
    }

    // GET /api/bookings/user/{userId} - list all sessions a user has booked
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<BookingDto>>> GetBookingsByUser(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("User not found.");

        var bookings = await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Session)
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.Session.Date)
            .ThenBy(b => b.Session.Time)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                UserId = b.UserId,
                Username = b.User.Username,
                FirstName = b.User.FirstName,
                LastName = b.User.LastName,
                SessionId = b.SessionId,
                SessionName = b.Session.Name,
                SessionDate = b.Session.Date,
                SessionTime = b.Session.Time,
                BookedAt = b.BookedAt
            })
            .ToListAsync();

        return Ok(bookings);
    }

    // POST /api/bookings - sign up for a session
    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingRequest request)
    {
        // Read userId from the JWT token, not from the request body
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("User not found.");

        if (user.IsDisabled)
            return BadRequest("This account has been disabled.");

        var session = await _context.Sessions
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId);

        if (session == null)
            return NotFound("Session not found.");

        if (session.Bookings.Count >= session.TotalSpots)
            return BadRequest("No spots available for this session.");

        // ✅ use userId from token, not request.UserId
        var existingBooking = await _context.Bookings
            .AnyAsync(b => b.UserId == userId && b.SessionId == request.SessionId);

        if (existingBooking)
            return BadRequest("User is already booked for this session.");

        // ✅ use userId from token, not request.UserId
        var booking = new Booking
        {
            UserId = userId,
            SessionId = request.SessionId
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        await _context.Entry(booking).Reference(b => b.User).LoadAsync();
        await _context.Entry(booking).Reference(b => b.Session).LoadAsync();

        return CreatedAtAction(nameof(GetBookingsBySession), new { sessionId = booking.SessionId }, new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            Username = booking.User.Username,
            FirstName = booking.User.FirstName,
            LastName = booking.User.LastName,
            SessionId = booking.SessionId,
            SessionName = booking.Session.Name,
            SessionDate = booking.Session.Date,
            SessionTime = booking.Session.Time,
            BookedAt = booking.BookedAt
        });
    }

    // DELETE /api/bookings/{id} - cancel a booking
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return NotFound();

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

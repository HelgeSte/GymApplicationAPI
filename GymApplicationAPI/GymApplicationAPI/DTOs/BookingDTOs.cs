namespace GymApplicationAPI.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int SessionId { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public DateOnly SessionDate { get; set; }
    public TimeOnly SessionTime { get; set; }
    public DateTime BookedAt { get; set; }
}

public class CreateBookingRequest
{
    public int UserId { get; set; }
    public int SessionId { get; set; }
}
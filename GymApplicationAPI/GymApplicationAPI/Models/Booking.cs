namespace GymApplicationAPI.Models;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SessionId { get; set; }
    public DateTime BookedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Session Session { get; set; } = null!;
}

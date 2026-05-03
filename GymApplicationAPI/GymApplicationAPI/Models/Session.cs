namespace GymApplicationAPI.Models;

public class Session
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public int TotalSpots { get; set; }
    public string Color { get; set; } = "bg-blue-200";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    // Computed - not stored in DB
    public int AvailableSpots => TotalSpots - Bookings.Count;
}

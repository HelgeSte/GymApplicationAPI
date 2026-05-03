namespace GymApplicationAPI.DTOs;

public class SessionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public int TotalSpots { get; set; }
    public int AvailableSpots { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class CreateSessionRequest
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public int TotalSpots { get; set; }
    public string Color { get; set; } = "bg-blue-200";
}

public class UpdateSessionRequest
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public int TotalSpots { get; set; }
    public string Color { get; set; } = string.Empty;
}
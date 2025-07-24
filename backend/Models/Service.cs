/// <summary>
/// Represents a service provided by the barbershop
/// </summary>
public class Service
{
    /// <summary>
    /// Unique identifier for the service
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the service
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Duration of the service in minutes
    /// </summary>
    public string DurationMinutes { get; set; } = string.Empty;

    /// <summary>
    /// Description of the service (optional)
    /// </summary>
    public string? Description { get; set; } = null;

    /// <summary>
    /// Price of the service (optional)
    /// </summary>
    public decimal? Price { get; set; }
}


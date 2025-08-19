using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Duration of the service in minutes
    /// </summary>
    [Required(ErrorMessage = "Duration is required")]
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Description of the service (optional)
    /// </summary>
    public string? Description { get; set; } = null;

    /// <summary>
    /// Price of the service (optional)
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? Price { get; set; }
}


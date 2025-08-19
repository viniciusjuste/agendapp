using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a service model.
/// </summary>
public class ServiceDto
{
    /// <summary>
    /// Gets or sets the name of the service.
    /// </summary>
    [MaxLength(50, ErrorMessage = "Name must be 50 characters or less")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the duration of the service in minutes.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
    public int? DurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets the description of the service.
    /// </summary>
    [MaxLength(200, ErrorMessage = "Description must be 200 characters or less")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the price of the service.
    /// </summary>
    public decimal? Price { get; set; }
}

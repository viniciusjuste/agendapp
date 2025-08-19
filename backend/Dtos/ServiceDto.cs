using System.ComponentModel.DataAnnotations;

public class ServiceDto
{
    public string? Name { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? Price { get; set; }
}
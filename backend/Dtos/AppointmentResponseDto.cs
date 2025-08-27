/// <summary>
/// Represents a response from the API for an appointment.
/// </summary>
public class AppointmentResponseDto
{
    /// <summary>
    /// The ID of the appointment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the user who made the appointment.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The name of the user who made the appointment.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The ID of the service that was booked.
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// The name of the service that was booked.
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// The date and time of the appointment.
    /// </summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// The status of the appointment.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Any notes added to the appointment.
    /// </summary>
    public string? Notes { get; set; }
}


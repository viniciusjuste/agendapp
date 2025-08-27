/// <summary>
/// Dto for creating a new appointment
/// </summary>
public class CreateAppointmentDto
{
    /// <summary>
    /// The ID of the service for which the appointment is being made
    /// </summary>
    public int ServiceId { get; set; }
    /// <summary>
    /// The date and time of the appointment
    /// </summary>
    public DateTime AppointmentDate { get; set; }
    /// <summary>
    /// Any additional notes or comments about the appointment
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}

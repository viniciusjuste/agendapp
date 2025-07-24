using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents an appointment made by a user
/// </summary>
public class Appointment
{
    /// <summary>
    /// The unique identifier of the appointment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The identifier of the user who made the appointment
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The user who made the appointment
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// The identifier of the service for which the appointment was made
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// The service for which the appointment was made
    /// </summary>
    [ForeignKey(nameof(ServiceId))]
    public Service Service { get; set; } = null!;

    /// <summary>
    /// The date and time of the appointment
    /// </summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// The status of the appointment
    /// </summary>
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    /// <summary>
    /// Additional notes about the appointment
    /// </summary>
    public string? Notes { get; set; }
}


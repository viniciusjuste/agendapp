/// <summary>
/// Represents a blocked schedule period.
/// </summary>
public class BlockedSchedule
{
    /// <summary>
    /// Gets or sets the unique identifier for the blocked schedule.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the blocked schedule.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start date of the blocked schedule.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the blocked schedule.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the reason for the blocked schedule.
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Represents the working hours of a professional.
/// </summary>
public class WorkingHours
{
    /// <summary>
    /// Gets or sets the ID of the working hours.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the day of the week.
    /// </summary>
    public DayOfWeek Day { get; set; }

    /// <summary>
    /// Gets or sets the start time of the working hours.
    /// </summary>
    public TimeSpan Start { get; set; }

    /// <summary>
    /// Gets or sets the end time of the working hours.
    /// </summary>
    public TimeSpan End { get; set; }
}


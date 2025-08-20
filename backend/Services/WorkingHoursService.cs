using Microsoft.EntityFrameworkCore;

public class WorkingHoursService
{
    private readonly AppDbContext _context;

    public WorkingHoursService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves available slots for a given date and service.
    /// </summary>
    /// <param name="date">The date to search for available slots. (YYYY-MM-DD)</param>
    /// <param name="serviceId">The service ID to search for available slots.</param>
    /// <returns>A list of available slots for the given date and service.</returns>
    /// <remarks>
    /// This method first searches for the service and duration, then gets the work hours for the day of the week.
    /// It then gets all appointments and blocked schedules for the day and generates available slots by
    /// checking for each 15-minute interval if there are any appointments or blocked schedules.
    /// </remarks>
    public List<DateTime> GetAvailableSlots(DateTime date, int serviceId)
    {
        var slots = new List<DateTime>();

        // 1. Search for the service and duration
        var service = _context.Services.FirstOrDefault(s => s.Id == serviceId);
        if (service == null) return slots;

        var duration = TimeSpan.FromMinutes(service.DurationMinutes);

        // 2. Get work hours for the day of the week
        var daySchedule = _context.WorkingHours
            .FirstOrDefault(w => w.Day == date.DayOfWeek);

        if (daySchedule == null) return slots; // dia sem atendimento

        // 3. Get appointments and blocked schedules for the day
        var appointments = _context.Appointments
            .Where(a => a.AppointmentDate.Date == date.Date && a.Status != AppointmentStatus.Canceled)
            .Include(a => a.Service)
            .ToList();

        var blockedSchedules = _context.BlockedSchedules
            .Where(b => b.StartDate.Date <= date.Date && b.EndDate.Date >= date.Date)
            .ToList();

        // 4. Generate available slots
        var current = date.Date.Add(daySchedule.Start);
        var endOfDay = date.Date.Add(daySchedule.End);

        while (current.Add(duration) <= endOfDay)
        {
            var slotEnd = current.Add(duration);

            bool isBlocked = blockedSchedules.Any(b =>
                current < b.EndDate && slotEnd > b.StartDate);

            bool isTaken = appointments.Any(a =>
                current < a.AppointmentDate.AddMinutes(a.Service.DurationMinutes) &&
                slotEnd > a.AppointmentDate);

            if (!isBlocked && !isTaken)
                slots.Add(current);

            current = current.AddMinutes(15); // interval between slots
        }
        return slots;
    }
}
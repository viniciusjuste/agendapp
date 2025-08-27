using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<Authentication> _logger;
        private readonly AuthService _authService;
        private readonly WorkingHoursService _workingHoursService;

        public AppointmentsController(AppDbContext context, ILogger<Authentication> logger, AuthService authService, WorkingHoursService workingHoursService)
        {
            _context = context;
            _logger = logger;
            _authService = authService;
            _workingHoursService = workingHoursService;
        }

        /// <summary>
        /// Retrieves available slots for a given date and service.
        /// </summary>
        /// <param name="date">The date to search for available slots. (YYYY-MM-DD)</param>
        /// <param name="serviceId">The service ID to search for available slots.</param>
        /// <returns>A list of available slots for the given date and service in the format of "HH:mm".</returns>
        /// <remarks>
        /// This endpoint calls the GetAvailableSlots method and formats the times to display in the front (e.g. "09:00").
        /// </remarks>
        [HttpGet("available")]
        public IActionResult GetAvailableSlotsEndPoint(DateTime date, int serviceId)
        {
            var slots = _workingHoursService.GetAvailableSlots(date, serviceId);

            // Format the times to display in the front (e.g. "09:00")
            var formattedSlots = slots.Select(d => d.ToString("HH:mm", CultureInfo.InvariantCulture)).ToList();

            return Ok(formattedSlots);
        }

        /// <summary>
        /// Retrieves a specific appointment by its ID.
        /// </summary>
        /// <param name="id">The ID of the appointment to retrieve.</param>
        /// <returns>An IActionResult containing the appointment if found, or an error message if not found or if an error occurred.</returns>
        /// <remarks>
        /// Queries the database for an appointment with the specified ID and returns it as an Appointment model.
        /// Logs errors in case of exceptions and returns a 500 status code.
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await _workingHoursService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }
            return Ok(appointment);
        }

        /// <summary>
        /// Creates a new appointment in the system.
        /// </summary>
        /// <param name="appointment">The appointment model containing the details of the appointment to be created.</param>
        /// <returns>An IActionResult containing the created appointment if successful, or an error message if not successful.</returns>
        /// <remarks>
        /// Validates the input appointment model and adds it to the database if valid. Logs warnings for null or invalid models.
        /// Logs errors in case of exceptions and returns a 500 status code.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
        {
            if (appointment.AppointmentDate <= DateTime.Now)
            {
                return BadRequest("Appointment date must be in the future.");
            }

            var user = await _context.Users.FindAsync(appointment.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var service = await _context.Services.FindAsync(appointment.ServiceId);
            if (service == null)
            {
                return NotFound("Service not found.");
            }

            var availableSlots = _workingHoursService.GetAvailableSlots(appointment.AppointmentDate.Date, appointment.ServiceId);

            if (!availableSlots.Any(a => a.TimeOfDay == appointment.AppointmentDate.TimeOfDay))
            {
                return BadRequest("The selected time slot is not available.");
            }

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }

    }
}

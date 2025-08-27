using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
        /// <param name="dto">The appointment model containing the details of the appointment to be created.</param>
        /// <returns>An IActionResult indicating the result of the creation operation, including success or error messages.</returns>
        /// <remarks>
        /// Validates the input appointment model and adds it to the database if valid. Logs warnings for null or invalid models.
        /// Logs errors in case of exceptions and returns a 500 status code.
        /// </remarks>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User not logged in.");

            int userId = int.Parse(userIdClaim);

            if (dto.AppointmentDate <= DateTime.Now)
                return BadRequest("Appointment date must be in the future.");

            var service = await _context.Services.FindAsync(dto.ServiceId);
            if (service == null)
                return NotFound("Service not found.");

            var availableSlots = _workingHoursService.GetAvailableSlots(dto.AppointmentDate.Date, dto.ServiceId);
            if (!availableSlots.Any(s => s.TimeOfDay == dto.AppointmentDate.TimeOfDay))
                return BadRequest("The selected time slot is not available.");

            var appointment = new Appointment
            {
                UserId = userId,
                ServiceId = dto.ServiceId,
                AppointmentDate = dto.AppointmentDate,
                Notes = dto.Notes,
                Status = AppointmentStatus.Pending
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            var response = new AppointmentResponseDto
            {
                Id = appointment.Id,
                UserId = appointment.UserId,
                UserName = user?.Name,  
                ServiceId = appointment.ServiceId,
                ServiceName = service.Name,
                AppointmentDate = appointment.AppointmentDate,
                Status = appointment.Status.ToString(),
                Notes = appointment.Notes
            };

            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, response);
        }
    }
}

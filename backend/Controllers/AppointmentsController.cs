using System.Globalization;
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
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<Authentication> _logger;

        private readonly AuthService _authService;

        public ServicesController(AppDbContext context, ILogger<Authentication> logger, AuthService authService)
        {
            _context = context;
            _logger = logger;
            _authService = authService;
        }

        /// <summary>
        /// Creates a new service in the system.
        /// </summary>
        /// <param name="serviceModel">The service model containing details of the service to be created.</param>
        /// <returns>An IActionResult indicating the result of the creation operation, including success or error messages.</returns>
        /// <remarks>
        /// Validates the input service model and adds it to the database if valid. Logs warnings for null or invalid models.
        /// Logs errors in case of exceptions and returns a 500 status code.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] Service serviceModel)
        {
            if (serviceModel == null)
            {
                _logger.LogWarning("Received null service model.");
                return BadRequest("Service model cannot be null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid service model received.");
                return BadRequest(ModelState);
            }

            try
            {
                _context.Services.Add(serviceModel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Service created successfully with ID: {ServiceId}", serviceModel.Id);

                return Ok(new
                {
                    message = "Service created successfully",
                    id = serviceModel.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a service.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves a list of all services in the system.
        /// </summary>
        /// <returns>An IActionResult containing a list of services if successful, or an error message if unsuccessful.</returns>
        /// <remarks>
        /// Queries the database for all records in the Services table and returns them as a list.
        /// Logs errors in case of exceptions and returns a 500 status code.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            try
            {
                var services = await _context.Services.ToListAsync();

                if (services == null || !services.Any())
                    return NoContent();

                return Ok(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving services.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceDto serviceDto)
        {
            if (serviceDto == null)
            {
                _logger.LogWarning("Received null service model.");
                return BadRequest("Service model cannot be null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid service model received.");
                return BadRequest(ModelState);
            }

            try
            {
                var service = await _context.Services.FindAsync(id);

                if (service == null)
                {
                    _logger.LogWarning("Service with ID {ServiceId} not found.", id);
                    return NotFound($"Service with ID {id} not found.");
                }

                if (!string.IsNullOrEmpty(serviceDto.Name))
                    service.Name = serviceDto.Name;

                if (serviceDto.DurationMinutes.HasValue)
                    service.DurationMinutes = serviceDto.DurationMinutes.Value;

                if (!string.IsNullOrEmpty(serviceDto.Description))
                    service.Description = serviceDto.Description;

                if (serviceDto.Price.HasValue)
                {
                    if (serviceDto.Price.Value < 0.01m)
                        return BadRequest("Price must be greater than 0");

                    service.Price = serviceDto.Price.Value;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Service updated successfully with ID: {ServiceId}", id);

                return Ok(new
                {
                    message = "Service updated successfully",
                    id = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating a service.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}

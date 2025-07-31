using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class Services : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<Authentication> _logger;

        private readonly AuthService _authService;

        public Services(AppDbContext context, ILogger<Authentication> logger, AuthService authService)
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
        public async Task<IActionResult> Post([FromBody] Service serviceModel)
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
    }
}

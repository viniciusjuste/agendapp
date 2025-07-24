using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authentication : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<Authentication> _logger;

        public Authentication(AppDbContext context, ILogger<Authentication> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="userRegisterDto">Object containing the user's registration details.</param>
        /// <returns>Result of the registration process (IActionResult).</returns>
        /// <remarks>
        /// Checks if the user's email already exists. If not, hashes the password and creates a new user record in the database.
        /// In case of an error, logs the exception and returns a 500 status code.
        /// </remarks>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            try
            {
                var userExists = await _context.Users
                .AnyAsync(u => u.Email == userRegisterDto.Email);

                if (userExists)
                {
                    return BadRequest("User already exists.");
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);

                var user = new User
                {
                    Email = userRegisterDto.Email,
                    Name = userRegisterDto.Username,
                    PasswordHash = hashedPassword,
                    Type = userRegisterDto.UserType

                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while registering the user.");
            }
        }
    }
}

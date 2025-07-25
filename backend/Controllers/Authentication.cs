using Microsoft.AspNetCore.Authorization;
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

        private readonly AuthService _authService;

        public Authentication(AppDbContext context, ILogger<Authentication> logger, AuthService authService)
        {
            _context = context;
            _logger = logger;
            _authService = authService;
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

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <param name="dto">Object containing the user's login credentials.</param>
        /// <returns>An IActionResult containing the JWT token if authentication is successful, or an error message if unsuccessful.</returns>
        /// <remarks>
        /// Checks if the user exists and verifies the password. If valid, generates a JWT token.
        /// Logs exceptions and returns a 500 status code in case of an error.
        /// </remarks>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                    return Unauthorized("Invalid credentials.");

                var token = _authService.GenerateJwtToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in for user {Email}.", dto.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while logging in for user {dto.Email}.");
            }
        }
    }
}

/// <summary>
/// Represents a user to be registered.
/// </summary>
public class UserRegisterDto
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user type.
    /// </summary>
    public UserType UserType { get; set; } = UserType.Standard;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

}

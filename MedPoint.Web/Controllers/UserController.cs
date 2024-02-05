using MedPoint.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MedPoint.Application.Interfaces;
using MedPoint.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MedPoint.Web.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public UserController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">The user login request.</param>
    /// <returns>A JWT token for the authenticated user.</returns>
    [HttpPost("authenticate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Authenticate([FromBody] UserLoginRequest request)
    {
        var user = await _userService.AuthenticateAsync(request.Username, request.Password);

        if (user == null)
            return BadRequest(new { message = "Username or password is incorrect" });

        var token = GenerateJwtToken(user);

        return Ok(new { user.Id, user.Username, user.Email, user.Role, Token = token });
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The user registration request.</param>
    /// <returns>The created user.</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
    {
        try
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Name = request.Name
            };

            var createdUser = await _userService.RegisterAsync(user, request.Password);
            //return Ok(createdUser);
            return Ok(new { createdUser.Id, createdUser.Username, createdUser.Email });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Sets the role of a user. Only accessible by admins.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="request">The role setting request.</param>
    /// <returns>A response indicating success or failure.</returns>
    [HttpPost("set-role/{userId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SetUserRole(Guid userId, [FromBody] SetUserRoleRequest request)
    {
        var success = await _userService.SetUserRoleAsync(userId, request.Role);
        if (success)
        {
            return Ok();
        }
        else
        {
            return BadRequest("Failed to set user role.");
        }
    }

    /// <summary>
    /// Lists users with pagination.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A list of users and the total count.</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (users, totalItems) = await _userService.ListUsersAsync(pageNumber, pageSize);
        return Ok(new { users, totalItems });
    }
}
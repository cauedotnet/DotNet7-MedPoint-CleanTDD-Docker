using MedPoint.Application.Interfaces;
using MedPoint.Domain.Interfaces;
using MedPoint.Domain.Entities;

namespace MedPoint.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> AuthenticateAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null; // User not found or password does not match
        }

        return user; // Authentication successful
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        return user;
    }

    public async Task<User> RegisterAsync(User user, string password)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User already exists.");
        }

        user.Role = "Reader"; // Sets the initial role (bussiness rule)

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        await _userRepository.CreateAsync(user);

        return user;
    }
    public async Task<bool> SetUserRoleAsync(Guid userId, string role)
    {
        // Additional permission checks can be performed here if needed
        return await _userRepository.SetUserRoleAsync(userId, role);
    }
    public async Task<(IEnumerable<User>, long)> ListUsersAsync(int pageNumber, int pageSize)
    {
        return await _userRepository.ListAsync(pageNumber, pageSize);
    }
}
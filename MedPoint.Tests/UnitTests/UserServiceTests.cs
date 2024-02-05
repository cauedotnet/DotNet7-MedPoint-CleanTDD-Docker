using MedPoint.Application.Services;
using MedPoint.Domain.Entities;
using MedPoint.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Tests.UnitTests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
    }

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsUser()
    {
        // Arrange
        var testUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("testpass")
        };

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync("testuser")).ReturnsAsync(testUser);

        // Act
        var result = await _userService.AuthenticateAsync("testuser", "testpass");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidPassword_ReturnsNull()
    {
        // Arrange
        var testUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("testpass")
        };

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync("testuser")).ReturnsAsync(testUser);

        // Act
        var result = await _userService.AuthenticateAsync("testuser", "wrongpass");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_NewUser_ReturnsUser()
    {
        // Arrange
        var newUser = new User
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Name = "New User"
        };

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync("newuser")).ReturnsAsync(() => null);
        _mockUserRepository.Setup(repo => repo.CreateAsync(It.IsAny<User>())).ReturnsAsync((User user) => user); 

        // Act
        var result = await _userService.RegisterAsync(newUser, "newpass");

        // Assert
        Assert.NotNull(result);
        _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_UserExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingUser = new User
        {
            Username = "existuser",
            Email = "existuser@example.com",
            Name = "Exist User"
        };

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync("existuser")).ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterAsync(existingUser, "existpass"));
    }

    [Fact]
    public async Task GetUserByIdAsync_ValidUserId_ReturnsUser()
    {
        // Arrange
        var testUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("testpass")
        };

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(testUser.Id)).ReturnsAsync(testUser);

        // Act
        var result = await _userService.GetUserByIdAsync(testUser.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testUser.Username, result.Username);
    }

    [Fact]
    public async Task GetUserByIdAsync_InvalidUserId_ReturnsNull()
    {
        // Arrange
        Guid invalidUserId = Guid.NewGuid();
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(invalidUserId)).ReturnsAsync(() => null);

        // Act
        var result = await _userService.GetUserByIdAsync(invalidUserId);

        // Assert
        Assert.Null(result);
    }

}
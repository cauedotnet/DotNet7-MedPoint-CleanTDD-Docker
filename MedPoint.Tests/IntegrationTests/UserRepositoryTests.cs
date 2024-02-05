using MongoDB.Driver;
using MedPoint.Domain.Entities;
using MedPoint.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Tests.IntegrationTests;

[Trait("Category", "IntegrationTests")]
public class UserRepositoryTests : IDisposable
{
    private readonly MongoDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        // Configure this to match your MongoDB test instance
        _context = new MongoDbContext("mongodb://localhost:27017", "MedPoint_TestDB_Users");
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_AddsUserSuccessfully()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testCreateUser",
            Name = "Test Create",
            Email = "create@test.com",
            PasswordHash = "hash",
            Role = "User"
        };

        var createdUser = await _repository.CreateAsync(user);

        Assert.NotNull(createdUser);
        var fetchedUser = await _context.Users.Find(x => x.Id == user.Id).FirstOrDefaultAsync();
        Assert.Equal("testCreateUser", fetchedUser.Username);

        // Cleanup
        await _context.Users.DeleteOneAsync(x => x.Id == user.Id);
    }

    [Fact]
    public async Task GetByUsernameAsync_ReturnsCorrectUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testGetByUsername",
            Name = "Test GetByUsername",
            Email = "getbyusername@test.com",
            PasswordHash = "hash",
            Role = "User"
        };

        await _context.Users.InsertOneAsync(user);

        var fetchedUser = await _repository.GetByUsernameAsync(user.Username);
        Assert.NotNull(fetchedUser);
        Assert.Equal("testGetByUsername", fetchedUser.Username);

        // Cleanup
        await _context.Users.DeleteOneAsync(x => x.Id == user.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testGetById",
            Name = "Test GetById",
            Email = "getbyid@test.com",
            PasswordHash = "hash",
            Role = "User"
        };

        await _context.Users.InsertOneAsync(user);

        var fetchedUser = await _repository.GetByIdAsync(user.Id);
        Assert.NotNull(fetchedUser);
        Assert.Equal(user.Id, fetchedUser.Id);

        // Cleanup
        await _context.Users.DeleteOneAsync(x => x.Id == user.Id);
    }

    public void Dispose()
    {
        // Drop the test database after each test
        _context.Client.DropDatabase("MedPoint_TestDB_Users");
    }
}
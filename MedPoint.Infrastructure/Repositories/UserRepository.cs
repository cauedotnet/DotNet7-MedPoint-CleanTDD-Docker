using MongoDB.Driver;
using MedPoint.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedPoint.Domain.Entities;

namespace MedPoint.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(MongoDbContext context)
    {
        _users = context.Users;
    }

    public async Task<User> CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> SetUserRoleAsync(Guid userId, string role)
    {
        var update = Builders<User>.Update.Set(u => u.Role, role);
        var result = await _users.UpdateOneAsync(u => u.Id == userId, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<(IEnumerable<User>, long)> ListAsync(int pageNumber, int pageSize)
    {
        var skip = (pageNumber - 1) * pageSize;
        var users = await _users.Find(_ => true)
                                .Skip(skip)
                                .Limit(pageSize)
                                .ToListAsync();
        var totalItems = await _users.CountDocumentsAsync(_ => true);

        return (users, totalItems);
    }
}
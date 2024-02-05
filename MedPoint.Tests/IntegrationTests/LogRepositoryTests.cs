using MongoDB.Driver;
using MedPoint.Domain.Entities;
using MedPoint.Domain.Enums;
using MedPoint.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Tests.IntegrationTests;

[Trait("Category", "IntegrationTests")]
public class LogRepositoryTests : IDisposable
{
    private readonly MongoDbContext _context;
    private readonly LogRepository _repository;

    public LogRepositoryTests()
    {
        // Configure this to match your MongoDB test instance
        _context = new MongoDbContext("mongodb://localhost:27017", "MedPoint_TestDB_Logs");
        _repository = new LogRepository(_context);
    }

    [Fact]
    public async Task LogAsync_InsertsLogSuccessfully()
    {
        var logEntry = new Log
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Action = LogAction.Create,
            UserId = Guid.NewGuid(),
            Entity = "TestEntity",
            EntityId = Guid.NewGuid(),
            Details = "Test details"
        };

        await _repository.LogAsync(logEntry);

        var savedLog = await _context.Logs.Find(x => x.Id == logEntry.Id).FirstOrDefaultAsync();
        Assert.NotNull(savedLog);
        Assert.Equal(logEntry.Details, savedLog.Details);

        // Cleanup
        await _context.Logs.DeleteOneAsync(x => x.Id == logEntry.Id);
    }

    [Fact]
    public async Task ListLatestLogsAsync_ReturnsLogsInCorrectOrder()
    {
        // Seed with multiple logs
        var logs = new List<Log>
        {
            new Log { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow.AddMinutes(-1), Action = LogAction.Create, UserId = Guid.NewGuid(), Entity = "Entity1", EntityId = Guid.NewGuid(), Details = "Details1" },
            new Log { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, Action = LogAction.Update, UserId = Guid.NewGuid(), Entity = "Entity2", EntityId = Guid.NewGuid(), Details = "Details2" }
        };

        await _context.Logs.InsertManyAsync(logs);

        var results = await _repository.ListLatestLogsAsync(2, 0);
        var resultList = results.ToList();

        Assert.Equal(2, resultList.Count);
        Assert.True(resultList[0].Timestamp >= resultList[1].Timestamp);

        // Cleanup
        foreach (var log in logs)
        {
            await _context.Logs.DeleteOneAsync(x => x.Id == log.Id);
        }
    }

    public void Dispose()
    {
        // Drop the test database after each test
        _context.Client.DropDatabase("MedPoint_TestDB_Logs");
    }
}
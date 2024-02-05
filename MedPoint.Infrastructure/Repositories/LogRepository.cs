using MongoDB.Driver;
using MedPoint.Domain.Entities;
using MedPoint.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Infrastructure.Repositories;

public class LogRepository : ILogRepository
{
    private readonly IMongoCollection<Log> _logs;

    public LogRepository(MongoDbContext context)
    {
        _logs = context.Logs;
    }

    public async Task LogAsync(Log logEntry)
    {
        await _logs.InsertOneAsync(logEntry);
    }

    public async Task<IEnumerable<Log>> ListLatestLogsAsync(int limit, int start)
    {
        return await _logs
                        .Find(_ => true)
                        .SortByDescending(log => log.Timestamp)
                        .Skip(start)
                        .Limit(limit)
                        .ToListAsync();
    }

}
using MedPoint.Application.Interfaces;
using MedPoint.Domain.Entities;
using MedPoint.Domain.Enums;
using MedPoint.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace MedPoint.Application.Services;

public class LogService : ILogService
{
    private readonly ILogRepository _logRepository;

    public LogService(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task LogActionAsync(LogAction action, Guid userId, string entity, Guid entityId, string details)
    {
        var logEntry = new Log
        {
            Timestamp = DateTime.UtcNow,
            Action = action,
            UserId = userId,
            Entity = entity,
            EntityId = entityId,
            Details = details
        };

        await _logRepository.LogAsync(logEntry);
    }

    public async Task<IEnumerable<Log>> ListLatestLogsAsync(int limit, int start)
    {
        return await _logRepository.ListLatestLogsAsync(limit, start);
    }
}
using MedPoint.Domain.Entities;
using MedPoint.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Application.Interfaces;

public interface ILogService
{
    Task LogActionAsync(LogAction action, Guid userId, string entity, Guid entityId, string details);
    Task<IEnumerable<Log>> ListLatestLogsAsync(int limit, int start);

}
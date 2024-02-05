using MedPoint.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Domain.Interfaces;

public interface ILogRepository
{
    Task LogAsync(Log logEntry);
    Task<IEnumerable<Log>> ListLatestLogsAsync(int limit, int start);
}
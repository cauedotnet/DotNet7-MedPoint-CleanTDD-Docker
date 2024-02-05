using MedPoint.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Domain.Entities;

public class Log
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public LogAction Action { get; set; }
    public Guid UserId { get; set; }
    public string Entity { get; set; }
    public Guid EntityId { get; set; }
    public string Details { get; set; }
}
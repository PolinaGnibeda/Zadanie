using HelpDesk.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Core.Services
{
    public class SlaCalculator
    {
        public DateTimeOffset CalculateSlaDeadline(DateTimeOffset createdAt, Priority priority)
        {
            return priority switch
            {
                Priority.Critical => createdAt.AddHours(4),
                Priority.High => createdAt.AddHours(8),
                Priority.Normal => createdAt.AddHours(24),
                Priority.Low => createdAt.AddHours(72),
                _ => throw new ArgumentOutOfRangeException(nameof(priority), "Неизвестный приоритет")
            };
        }
    }
}

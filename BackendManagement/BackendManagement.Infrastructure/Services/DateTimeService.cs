using BackendManagement.Application.Common.Interfaces;

namespace BackendManagement.Infrastructure.Services;

public class DateTimeService : Application.Common.Interfaces.IDateTime
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
} 
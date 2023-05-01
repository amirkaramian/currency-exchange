using Exchange.Application.Common.Interfaces;

namespace Exchange.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}

namespace Destination.Infrastructure.Common.Time;

using Application.Abstractions.Time;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

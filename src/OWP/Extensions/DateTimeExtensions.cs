namespace OWP.Core.Extensions;

public static class DateTimeExtensions
{
    private const int DefaultGmt8 = 8;

    public static DateTime ToUtc8(this DateTime date)
    {
        return date.AddHours(DefaultGmt8);
    }

    public static DateTime ToUtc0(this DateTime date)
    {
        return date.AddHours(-DefaultGmt8);
    }

    public static DateTime ToMax(this DateTime instance, DateTime dateTime)
    {
        return instance > dateTime ? instance : dateTime;
    }
}
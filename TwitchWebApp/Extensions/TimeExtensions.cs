namespace TwitchWebApp.Extensions;

public static class TimeExtensions
{
    public static TimeSpan StripMillisecons(this TimeSpan time)
    {
        return new TimeSpan(time.Hours, time.Minutes, time.Seconds);
    }
}
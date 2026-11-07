namespace PTView.SDK.Models;

/// <summary>
/// Represents a normalized chart timeframe interval.
/// </summary>
public readonly record struct TimeFrame : IEquatable<TimeFrame>, IComparable<TimeFrame>
{
    public static readonly TimeFrame OneMinute = new("1m", TimeSpan.FromMinutes(1));
    public static readonly TimeFrame ThreeMinutes = new("3m", TimeSpan.FromMinutes(3));
    public static readonly TimeFrame FiveMinutes = new("5m", TimeSpan.FromMinutes(5));
    public static readonly TimeFrame FifteenMinutes = new("15m", TimeSpan.FromMinutes(15));
    public static readonly TimeFrame ThirtyMinutes = new("30m", TimeSpan.FromMinutes(30));
    public static readonly TimeFrame OneHour = new("1h", TimeSpan.FromHours(1));
    public static readonly TimeFrame TwoHours = new("2h", TimeSpan.FromHours(2));
    public static readonly TimeFrame FourHours = new("4h", TimeSpan.FromHours(4));
    public static readonly TimeFrame SixHours = new("6h", TimeSpan.FromHours(6));
    public static readonly TimeFrame TwelveHours = new("12h", TimeSpan.FromHours(12));
    public static readonly TimeFrame OneDay = new("1d", TimeSpan.FromDays(1));
    public static readonly TimeFrame OneWeek = new("1w", TimeSpan.FromDays(7));

    public string Code { get; }
    public TimeSpan Duration { get; }
    public long DurationMs => (long)Duration.TotalMilliseconds;

    public TimeFrame(string code, TimeSpan duration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        if (duration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(duration), "Duration must be positive.");

        Code = code.Trim().ToLowerInvariant();
        Duration = duration;
    }

    public static TimeFrame FromCode(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return code.Trim().ToLowerInvariant() switch
        {
            "1m" => OneMinute,
            "3m" => ThreeMinutes,
            "5m" => FiveMinutes,
            "15m" => FifteenMinutes,
            "30m" => ThirtyMinutes,
            "1h" => OneHour,
            "2h" => TwoHours,
            "4h" => FourHours,
            "6h" => SixHours,
            "12h" => TwelveHours,
            "1d" => OneDay,
            "1w" => OneWeek,
            _ => ParseCustomCode(code)
        };
    }

    private static TimeFrame ParseCustomCode(string code)
    {
        string trimmed = code.Trim().ToLowerInvariant();
        if (trimmed.Length < 2)
            throw new FormatException($"Invalid timeframe code '{code}'.");

        char unit = trimmed[^1];
        if (!int.TryParse(trimmed[..^1], out int value) || value <= 0)
            throw new FormatException($"Invalid timeframe duration in '{code}'.");

        TimeSpan duration = unit switch
        {
            's' => TimeSpan.FromSeconds(value),
            'm' => TimeSpan.FromMinutes(value),
            'h' => TimeSpan.FromHours(value),
            'd' => TimeSpan.FromDays(value),
            'w' => TimeSpan.FromDays(value * 7),
            _ => throw new FormatException($"Unsupported timeframe unit '{unit}'.")
        };

        return new TimeFrame(trimmed, duration);
    }

    public int CompareTo(TimeFrame other) => Duration.CompareTo(other.Duration);

    public static bool operator <(TimeFrame left, TimeFrame right) => left.CompareTo(right) < 0;
    public static bool operator >(TimeFrame left, TimeFrame right) => left.CompareTo(right) > 0;
    public static bool operator <=(TimeFrame left, TimeFrame right) => left.CompareTo(right) <= 0;
    public static bool operator >=(TimeFrame left, TimeFrame right) => left.CompareTo(right) >= 0;

    public override string ToString() => Code;
}

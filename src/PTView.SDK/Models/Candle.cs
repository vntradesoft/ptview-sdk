namespace PTView.SDK.Models;

/// <summary>
/// Represents an immutable candlestick (bar) in a market data series.
/// All timestamps are represented in UTC Unix milliseconds.
/// </summary>
public readonly record struct Candle : IEquatable<Candle>
{
    /// <summary>
    /// Candle open timestamp in UTC Unix milliseconds.
    /// </summary>
    public long OpenTimeMs { get; init; }

    /// <summary>
    /// Opening price of the period.
    /// </summary>
    public decimal Open { get; init; }

    /// <summary>
    /// Highest price observed during the period.
    /// </summary>
    public decimal High { get; init; }

    /// <summary>
    /// Lowest price observed during the period.
    /// </summary>
    public decimal Low { get; init; }

    /// <summary>
    /// Closing price (or current price if forming).
    /// </summary>
    public decimal Close { get; init; }

    /// <summary>
    /// Base currency volume traded during the period.
    /// </summary>
    public decimal Volume { get; init; }

    /// <summary>
    /// True if this candle period is finalized/closed; false if it is currently forming.
    /// </summary>
    public bool IsClosed { get; init; }

    /// <summary>
    /// Calculated close timestamp in UTC Unix milliseconds based on the timeframe duration.
    /// </summary>
    public long CloseTimeMs(long durationMs) => OpenTimeMs + durationMs - 1;

    public Candle(
        long openTimeMs,
        decimal open,
        decimal high,
        decimal low,
        decimal close,
        decimal volume,
        bool isClosed = true)
    {
        if (openTimeMs < 0)
            throw new ArgumentOutOfRangeException(nameof(openTimeMs), "Timestamp must be non-negative.");

        OpenTimeMs = openTimeMs;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
        IsClosed = isClosed;
    }

    /// <summary>
    /// Returns the candle OpenTime as a UTC DateTime.
    /// </summary>
    public DateTime OpenTimeUtc => DateTimeOffset.FromUnixTimeMilliseconds(OpenTimeMs).UtcDateTime;

    /// <summary>
    /// Returns a new instance updating the latest price and volume for a forming candle.
    /// </summary>
    public Candle UpdateTick(decimal price, decimal addedVolume, bool isClosed = false)
    {
        decimal newHigh = price > High ? price : High;
        decimal newLow = price < Low ? price : Low;
        return new Candle(OpenTimeMs, Open, newHigh, newLow, price, Volume + addedVolume, isClosed);
    }
}

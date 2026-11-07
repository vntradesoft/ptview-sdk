using System.Runtime.InteropServices;

namespace PTView.SDK.Models;

/// <summary>
/// Memory-optimized 48-byte candlestick representation designed for high-throughput
/// binary history storage, streaming, and direct GPU/WebView2 charting.
/// </summary>
/// <remarks>
/// Field layout:
///   [0..7]   Int64  - OpenTimeMs in UTC Unix milliseconds (8 bytes)
///   [8..15]  Double - Open price (8 bytes)
///   [16..23] Double - High price (8 bytes)
///   [24..31] Double - Low price (8 bytes)
///   [32..39] Double - Close price (8 bytes)
///   [40..47] Double - Traded volume (8 bytes)
/// Total: 48 bytes (naturally 8-byte aligned, zero padding, zero managed heap allocation).
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct CompactCandle : IEquatable<CompactCandle>
{
    /// <summary>Fixed byte size of a serialized CompactCandle record on disk and in memory.</summary>
    public const int SizeInBytes = 48;

    /// <summary>Candle open timestamp in UTC Unix milliseconds.</summary>
    public long OpenTimeMs { get; init; }

    /// <summary>Opening price.</summary>
    public double Open { get; init; }

    /// <summary>Highest price.</summary>
    public double High { get; init; }

    /// <summary>Lowest price.</summary>
    public double Low { get; init; }

    /// <summary>Closing price.</summary>
    public double Close { get; init; }

    /// <summary>Traded volume in base currency.</summary>
    public double Volume { get; init; }

    /// <summary>
    /// Initializes a new instance of <see cref="CompactCandle"/>.
    /// </summary>
    public CompactCandle(long openTimeMs, double open, double high, double low, double close, double volume)
    {
        OpenTimeMs = openTimeMs;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
    }

    /// <summary>Gets the candle OpenTime as a UTC DateTime.</summary>
    public DateTime OpenTimeUtc => DateTimeOffset.FromUnixTimeMilliseconds(OpenTimeMs).UtcDateTime;

    /// <summary>Converts this compact candle to an SDK <see cref="Candle"/> struct.</summary>
    public Candle ToCandle(bool isClosed = true) =>
        new(OpenTimeMs, (decimal)Open, (decimal)High, (decimal)Low, (decimal)Close, (decimal)Volume, isClosed);

    /// <summary>Implicit conversion from SDK <see cref="Candle"/> to <see cref="CompactCandle"/>.</summary>
    public static implicit operator CompactCandle(Candle c) =>
        new(c.OpenTimeMs, (double)c.Open, (double)c.High, (double)c.Low, (double)c.Close, (double)c.Volume);
}


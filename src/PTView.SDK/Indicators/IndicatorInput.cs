using PTView.SDK.Models;

namespace PTView.SDK.Indicators;

/// <summary>
/// Encapsulates inputs passed to an indicator calculation pass.
/// </summary>
public sealed class IndicatorInput
{
    /// <summary>
    /// The full historical series of candles currently loaded.
    /// </summary>
    public IReadOnlyList<Candle> Candles { get; }

    /// <summary>
    /// The index in <see cref="Candles"/> being calculated or updated.
    /// In full calculation, passes 0..N-1 sequentially.
    /// In incremental calculation, points to the current active/updated candle index.
    /// </summary>
    public int CurrentIndex { get; }

    /// <summary>
    /// True if only the latest bar is being updated incrementally.
    /// </summary>
    public bool IsIncremental { get; }

    /// <summary>
    /// The timeframe of the series.
    /// </summary>
    public TimeFrame TimeFrame { get; }

    public IndicatorInput(
        IReadOnlyList<Candle> candles,
        int currentIndex,
        bool isIncremental,
        TimeFrame timeFrame)
    {
        Candles = candles ?? throw new ArgumentNullException(nameof(candles));
        CurrentIndex = currentIndex;
        IsIncremental = isIncremental;
        TimeFrame = timeFrame;
    }

    /// <summary>
    /// Convenience helper to retrieve candle at relative bar offset (0 = current, 1 = 1 bar ago, etc.).
    /// </summary>
    public Candle? GetCandle(int barsAgo = 0)
    {
        int targetIndex = CurrentIndex - barsAgo;
        if (targetIndex >= 0 && targetIndex < Candles.Count)
            return Candles[targetIndex];
        return null;
    }
}


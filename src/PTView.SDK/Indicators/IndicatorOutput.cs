namespace PTView.SDK.Indicators;

/// <summary>
/// Encapsulates the output buffers created and updated by an indicator.
/// Supports multiple named plots (e.g., MACD line, Signal line, Histogram) and auxiliary calculation/color buffers.
/// </summary>
public sealed class IndicatorOutput
{
    private readonly Dictionary<string, IIndicatorBuffer> _buffers = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, IIndicatorBuffer> Buffers => _buffers;

    public void RegisterBuffer(string plotId, IIndicatorBuffer buffer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plotId);
        _buffers[plotId] = buffer ?? throw new ArgumentNullException(nameof(buffer));
    }

    public IIndicatorBuffer RegisterBuffer(string plotId, IndicatorBufferType type = IndicatorBufferType.Data, int initialCapacity = 1024)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plotId);
        var buffer = new IndicatorBuffer(plotId, initialCapacity, type);
        _buffers[plotId] = buffer;
        return buffer;
    }

    public IIndicatorBuffer GetBuffer(string plotId)
    {
        if (_buffers.TryGetValue(plotId, out var buffer))
            return buffer;
        throw new KeyNotFoundException($"Buffer for plot '{plotId}' not registered.");
    }

    public bool TryGetBuffer(string plotId, out IIndicatorBuffer? buffer) =>
        _buffers.TryGetValue(plotId, out buffer);

    public void ClearAll()
    {
        foreach (var buffer in _buffers.Values)
        {
            buffer.Clear();
        }
    }
}

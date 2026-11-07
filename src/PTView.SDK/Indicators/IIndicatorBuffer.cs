namespace PTView.SDK.Indicators;

/// <summary>
/// Represents a time-series output buffer for indicator values aligned with candle bars.
/// </summary>
public interface IIndicatorBuffer
{
    string PlotId { get; }
    IndicatorBufferType BufferType { get; }
    int Count { get; }
    double? this[int index] { get; }
    double? GetLastValue();
    void SetValue(int index, double? value);
    void AppendValue(double? value);
    void UpdateLastValue(double? value);
    IReadOnlyList<double?> AsReadOnly();
    void Clear();
}

/// <summary>
/// Efficient list-based indicator output buffer.
/// </summary>
public sealed class IndicatorBuffer : IIndicatorBuffer
{
    private readonly List<double?> _values;

    public string PlotId { get; }
    public IndicatorBufferType BufferType { get; }

    public int Count => _values.Count;

    public double? this[int index]
    {
        get
        {
            if (index < 0 || index >= _values.Count)
                return null;
            return _values[index];
        }
    }

    public IndicatorBuffer(string plotId, int capacity = 1024, IndicatorBufferType bufferType = IndicatorBufferType.Data)
    {
        PlotId = plotId ?? throw new ArgumentNullException(nameof(plotId));
        BufferType = bufferType;
        _values = new List<double?>(capacity);
    }

    public double? GetLastValue() => _values.Count > 0 ? _values[^1] : null;

    public void SetValue(int index, double? value)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        while (_values.Count <= index)
        {
            _values.Add(null);
        }

        _values[index] = value;
    }

    public void AppendValue(double? value)
    {
        _values.Add(value);
    }

    public void UpdateLastValue(double? value)
    {
        if (_values.Count == 0)
        {
            _values.Add(value);
        }
        else
        {
            _values[^1] = value;
        }
    }

    public IReadOnlyList<double?> AsReadOnly() => _values.AsReadOnly();

    public void Clear() => _values.Clear();
}

namespace PTView.SDK.Indicators;

/// <summary>
/// Specifies the functional role of an indicator buffer.
/// </summary>
public enum IndicatorBufferType
{
    /// <summary>
    /// Primary data buffer that represents a visual plot series or public indicator output.
    /// </summary>
    Data = 0,

    /// <summary>
    /// Auxiliary buffer that stores color indices (e.g. 0, 1, 2) mapped to a color palette per candle bar.
    /// </summary>
    ColorIndex = 1,

    /// <summary>
    /// Internal scratchpad or intermediate calculation buffer (e.g. bands, true range).
    /// Not directly rendered on the chart or displayed in the Data Window.
    /// </summary>
    Calculations = 2
}


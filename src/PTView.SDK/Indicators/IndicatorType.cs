namespace PTView.SDK.Indicators;

/// <summary>
/// Specifies whether an indicator renders directly on the main price chart or in a separate pane below.
/// </summary>
public enum IndicatorType
{
    /// <summary>
    /// Renders overlaid directly onto the main price scale (e.g., Moving Averages, Bollinger Bands, Parabolic SAR).
    /// </summary>
    Overlay = 0,

    /// <summary>
    /// Renders in a separate sub-chart pane with its own independent scale (e.g., RSI, MACD, Stochastic).
    /// </summary>
    SeparatePane = 1
}

/// <summary>
/// Specifies the visual rendering style of an indicator plot series.
/// </summary>
public enum PlotStyle
{
    Line = 0,
    Histogram = 1,
    Points = 2,
    Area = 3,
    Columns = 4
}


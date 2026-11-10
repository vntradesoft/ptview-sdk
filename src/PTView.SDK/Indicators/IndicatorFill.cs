using PTView.SDK.Models;

namespace PTView.SDK.Indicators;

/// <summary>
/// Defines a filled area between two indicator plots (e.g., Ichimoku Kumo cloud, Bollinger Bands channel).
/// </summary>
public sealed class IndicatorFill
{
    public string Id { get; }
    public string DisplayName { get; }
    public string PlotId1 { get; }
    public string PlotId2 { get; }
    public ColorRgba Color { get; set; }

    /// <summary>
    /// Optional secondary color used when PlotId1 value is less than PlotId2 value
    /// (e.g. bearish Ichimoku cloud where Span A &lt; Span B).
    /// If null, <see cref="Color"/> is used across the entire fill regardless of relative order.
    /// </summary>
    public ColorRgba? BearishColor { get; set; }

    public bool IsVisible { get; set; }

    public IndicatorFill(
        string id,
        string displayName,
        string plotId1,
        string plotId2,
        ColorRgba color,
        ColorRgba? bearishColor = null,
        bool isVisible = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(plotId1);
        ArgumentException.ThrowIfNullOrWhiteSpace(plotId2);

        Id = id;
        DisplayName = displayName;
        PlotId1 = plotId1;
        PlotId2 = plotId2;
        Color = color;
        BearishColor = bearishColor;
        IsVisible = isVisible;
    }
}


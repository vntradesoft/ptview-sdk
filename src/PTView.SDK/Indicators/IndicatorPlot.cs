using PTView.SDK.Models;

namespace PTView.SDK.Indicators;

/// <summary>
/// Defines a visual plot series produced by an indicator (e.g., 'Signal Line', 'Histogram', 'Upper Band').
/// </summary>
public sealed class IndicatorPlot
{
    public string Id { get; }
    public string DisplayName { get; }
    public PlotStyle Style { get; }
    public ColorRgba Color { get; set; }
    public int LineWidth { get; set; }

    /// <summary>
    /// Optional buffer identifier that supplies per-bar color indices for dynamic segment coloring.
    /// </summary>
    public string? ColorIndexPlotId { get; set; }

    /// <summary>
    /// Optional color palette mapping integer color indices (0, 1, 2...) to colors.
    /// </summary>
    public IReadOnlyDictionary<int, ColorRgba>? ColorPalette { get; set; }

    public IndicatorPlot(
        string id,
        string displayName,
        PlotStyle style = PlotStyle.Line,
        ColorRgba? color = null,
        int lineWidth = 2,
        string? colorIndexPlotId = null,
        IReadOnlyDictionary<int, ColorRgba>? colorPalette = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        Id = id;
        DisplayName = displayName;
        Style = style;
        Color = color ?? ColorRgba.Blue;
        LineWidth = lineWidth;
        ColorIndexPlotId = colorIndexPlotId;
        ColorPalette = colorPalette;
    }
}

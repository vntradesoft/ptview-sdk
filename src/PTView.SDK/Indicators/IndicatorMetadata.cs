namespace PTView.SDK.Indicators;

/// <summary>
/// Static metadata describing an indicator definition, its visual plots, parameters,
/// and chart rendering requirements (compatible with standard indicator categories and guide levels).
/// </summary>
public sealed record IndicatorMetadata(
    string Id,
    string Name,
    string ShortName,
    string Description,
    IndicatorType Type,
    IReadOnlyList<IndicatorParameter> Parameters,
    IReadOnlyList<IndicatorPlot> Plots,
    string Category = "Trend",
    double? MinValue = null,
    double? MaxValue = null,
    IReadOnlyList<double>? GuideLevels = null,
    string IconKey = "Icon.IndicatorChart",
    IReadOnlyList<IndicatorFill>? Fills = null)
{
    public IReadOnlyList<IndicatorFill> Fills { get; init; } = Fills ?? Array.Empty<IndicatorFill>();

    public static IndicatorMetadata Create(
        string id,
        string name,
        string shortName,
        string description,
        IndicatorType type,
        IEnumerable<IndicatorParameter> parameters,
        IEnumerable<IndicatorPlot> plots,
        string category = "Trend",
        double? minValue = null,
        double? maxValue = null,
        IEnumerable<double>? guideLevels = null,
        string iconKey = "Icon.IndicatorChart",
        IEnumerable<IndicatorFill>? fills = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(shortName);

        return new IndicatorMetadata(
            id,
            name,
            shortName,
            description,
            type,
            parameters.ToList().AsReadOnly(),
            plots.ToList().AsReadOnly(),
            category,
            minValue,
            maxValue,
            guideLevels?.ToList().AsReadOnly(),
            iconKey,
            fills?.ToList().AsReadOnly());
    }
}

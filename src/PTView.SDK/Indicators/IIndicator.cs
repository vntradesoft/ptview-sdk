namespace PTView.SDK.Indicators;

/// <summary>
/// Core contract implemented by all PTView technical indicators (built-in and third-party plugins).
/// Completely independent of UI, chart rendering, and exchange connectivity libraries.
/// </summary>
public interface IIndicator : IDisposable
{
    /// <summary>
    /// Metadata defining the indicator's identification, plots, and parameters.
    /// </summary>
    IndicatorMetadata Metadata { get; }

    /// <summary>
    /// Initializes the indicator with runtime context and output buffers.
    /// </summary>
    void Initialize(IndicatorContext context);

    /// <summary>
    /// Resets all internal calculation states and buffers.
    /// </summary>
    void Reset();

    /// <summary>
    /// Calculates indicator values for a single bar pass.
    /// For full recalculation, the engine invokes this sequentially for each bar in the history.
    /// For real-time updates, the engine invokes this incrementally for the active bar only.
    /// </summary>
    /// <param name="input">Bar series input and index information.</param>
    void Calculate(IndicatorInput input);
}


using PTView.SDK.Indicators;

namespace PTView.SDK.Indicators;

/// <summary>
/// Convenient abstract base class for technical indicators providing common boilerplate,
/// automatic output buffer registration from <see cref="IndicatorMetadata.Plots"/>, and bar state management.
/// Available to both built-in indicators and third-party plugin assemblies.
/// </summary>
public abstract class IndicatorBase : IIndicator
{
    private readonly List<IIndicator> _childIndicators = [];
    private bool _disposed;

    public abstract IndicatorMetadata Metadata { get; }
    public IndicatorContext? Context { get; private set; }

    /// <summary>
    /// Gets a registered output buffer by its plot identifier.
    /// </summary>
    public IIndicatorBuffer? GetPlotBuffer(string plotId) => Context?.Output.GetBuffer(plotId);

    /// <summary>
    /// Gets the collection of registered child indicator instances scoped to this indicator.
    /// </summary>
    public IReadOnlyList<IIndicator> ChildIndicators => _childIndicators;

    /// <summary>
    /// Gets the primary output buffer (first plot defined in <see cref="IndicatorMetadata.Plots"/>) for this indicator.
    /// </summary>
    public IIndicatorBuffer? PrimaryOutputBuffer =>
        Metadata.Plots.Count > 0 && Context != null
            ? Context.Output.GetBuffer(Metadata.Plots[0].Id)
            : null;

    public virtual void Initialize(IndicatorContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        foreach (var plot in Metadata.Plots)
        {
            var buffer = new IndicatorBuffer(plot.Id, 1024, IndicatorBufferType.Data);
            context.Output.RegisterBuffer(plot.Id, buffer);

            if (!string.IsNullOrWhiteSpace(plot.ColorIndexPlotId) && !context.Output.TryGetBuffer(plot.ColorIndexPlotId, out _))
            {
                var colorBuffer = new IndicatorBuffer(plot.ColorIndexPlotId, 1024, IndicatorBufferType.ColorIndex);
                context.Output.RegisterBuffer(plot.ColorIndexPlotId, colorBuffer);
            }
        }
        OnInit();
    }

    public virtual void Reset()
    {
        Context?.Output.ClearAll();
        for (int i = 0; i < _childIndicators.Count; i++)
        {
            _childIndicators[i].Reset();
        }
        OnReset();
    }

    /// <summary>
    /// Creates and initializes a child indicator instance scoped to this parent indicator.
    /// Child indicators inherit the parent's timeframe and instance scoping, and are automatically reset and disposed with the parent.
    /// </summary>
    protected TChild CreateChildIndicator<TChild>(string subId, params IndicatorParameter[] parameters)
        where TChild : IIndicator, new()
    {
        if (Context == null)
            throw new InvalidOperationException("IndicatorContext has not been initialized yet.");

        var child = new TChild();
        return AttachChildIndicator(child, subId, parameters);
    }

    /// <summary>
    /// Attaches and initializes an existing child indicator instance scoped to this parent indicator.
    /// </summary>
    protected TChild AttachChildIndicator<TChild>(TChild child, string subId, params IndicatorParameter[] parameters)
        where TChild : IIndicator
    {
        if (Context == null)
            throw new InvalidOperationException("IndicatorContext has not been initialized yet.");

        ArgumentNullException.ThrowIfNull(child);

        var childContext = new IndicatorContext(
            $"{Context.InstanceId}_{subId}",
            Context.TimeFrame,
            parameters);

        child.Initialize(childContext);
        _childIndicators.Add(child);
        return child;
    }

    /// <summary>
    /// Registers an auxiliary buffer (e.g. ColorIndex or Calculations) with the current indicator context.
    /// </summary>
    protected IIndicatorBuffer RegisterBuffer(string bufferId, IndicatorBufferType type = IndicatorBufferType.Calculations, int capacity = 1024)
    {
        if (Context == null)
            throw new InvalidOperationException("IndicatorContext has not been initialized yet.");

        var buffer = new IndicatorBuffer(bufferId, capacity, type);
        Context.Output.RegisterBuffer(bufferId, buffer);
        return buffer;
    }

    /// <summary>
    /// Safely gets a parameter value from the runtime context, or returns the specified fallback default if not found.
    /// </summary>
    protected T GetParam<T>(string name, T defaultValue = default!)
    {
        return Context != null ? Context.GetParameterOrDefault(name, defaultValue) : defaultValue;
    }

    public abstract void Calculate(IndicatorInput input);

    protected virtual void OnInit() { }
    protected virtual void OnReset() { }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            for (int i = 0; i < _childIndicators.Count; i++)
            {
                _childIndicators[i].Dispose();
            }
            _childIndicators.Clear();
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

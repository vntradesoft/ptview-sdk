using PTView.SDK.Models;

namespace PTView.SDK.Indicators;

/// <summary>
/// Provides runtime context, parameter access, and state tracking to an indicator instance.
/// </summary>
public sealed class IndicatorContext
{
    private readonly Dictionary<string, IndicatorParameter> _parameters = new(StringComparer.OrdinalIgnoreCase);

    public string InstanceId { get; }
    public TimeFrame TimeFrame { get; }
    public IndicatorOutput Output { get; }

    public IndicatorContext(string instanceId, TimeFrame timeFrame, IEnumerable<IndicatorParameter> parameters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(instanceId);

        InstanceId = instanceId;
        TimeFrame = timeFrame;
        Output = new IndicatorOutput();

        if (parameters != null)
        {
            foreach (var p in parameters)
            {
                _parameters[p.Name] = p;
            }
        }
    }

    public T GetParameter<T>(string name)
    {
        if (_parameters.TryGetValue(name, out var param))
            return param.GetValue<T>();

        throw new KeyNotFoundException($"Parameter '{name}' not found on indicator.");
    }

    public bool TryGetParameter<T>(string name, out T value)
    {
        if (_parameters.TryGetValue(name, out var param))
        {
            value = param.GetValue<T>();
            return true;
        }

        value = default!;
        return false;
    }

    public T GetParameterOrDefault<T>(string name, T defaultValue = default!)
    {
        if (TryGetParameter<T>(name, out var value))
            return value;

        return defaultValue;
    }
}


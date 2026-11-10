namespace PTView.SDK.Indicators;

/// <summary>
/// Defines a configurable parameter exposed by an indicator (e.g. Period, Source, Multiplier).
/// </summary>
public sealed class IndicatorParameter
{
    public string Name { get; }
    public string DisplayName { get; }
    public string Description { get; }
    public Type ValueType { get; }
    public object? DefaultValue { get; }
    public object? MinValue { get; }
    public object? MaxValue { get; }
    public object? CurrentValue { get; set; }

    public IndicatorParameter(
        string name,
        string displayName,
        Type valueType,
        object? defaultValue,
        string description = "",
        object? minValue = null,
        object? maxValue = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        Name = name;
        DisplayName = displayName;
        ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
        DefaultValue = defaultValue;
        CurrentValue = defaultValue;
        Description = description;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public T GetValue<T>()
    {
        if (CurrentValue is T typedVal)
            return typedVal;

        if (CurrentValue == null)
            return default!;

        return (T)Convert.ChangeType(CurrentValue, typeof(T));
    }
}


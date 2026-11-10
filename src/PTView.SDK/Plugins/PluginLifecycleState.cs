namespace PTView.SDK.Plugins;

/// <summary>
/// Represents the current lifecycle state of a PTView plugin.
/// </summary>
public enum PluginLifecycleState
{
    Unloaded = 0,
    Loaded = 1,
    Initialized = 2,
    Active = 3,
    Disabled = 4,
    Failed = 5,
    Disposed = 6
}


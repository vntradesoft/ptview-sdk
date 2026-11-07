namespace PTView.SDK.Plugins;

/// <summary>
/// Runtime metadata for a loaded plugin instance.
/// </summary>
public sealed record PluginMetadata(
    string Id,
    string Name,
    Version Version,
    string Description,
    string Author,
    bool IsBuiltIn);


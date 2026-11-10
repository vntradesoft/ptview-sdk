using PTView.SDK.Indicators;

namespace PTView.SDK.Plugins;

/// <summary>
/// Host context provided to a plugin during initialization.
/// Allows registration of custom indicators without leaking internal platform runtime types.
/// </summary>
public interface IPluginContext
{
    /// <summary>
    /// Current SDK version of the hosting platform.
    /// </summary>
    Version SdkVersion { get; }

    /// <summary>
    /// Registers a technical indicator factory into the host runtime.
    /// </summary>
    void RegisterIndicator<TIndicator>() where TIndicator : class, IIndicator, new();

    /// <summary>
    /// Registers an indicator with an explicit factory delegate.
    /// </summary>
    void RegisterIndicator(IndicatorMetadata metadata, Func<IIndicator> factory);

    /// <summary>
    /// Logs an informational message from the plugin.
    /// </summary>
    void LogInfo(string message);

    /// <summary>
    /// Logs an error message from the plugin.
    /// </summary>
    void LogError(string message, Exception? exception = null);
}

/// <summary>
/// Root entry-point interface that every PTView plugin DLL must implement.
/// </summary>
public interface IPTViewPlugin : IDisposable
{
    /// <summary>
    /// Plugin manifest loaded from plugin metadata.
    /// </summary>
    PluginMetadata Metadata { get; }

    /// <summary>
    /// Invoked once when the plugin is loaded into the host environment.
    /// </summary>
    Task InitializeAsync(IPluginContext context, CancellationToken ct = default);

    /// <summary>
    /// Invoked when the plugin is enabled by the user or workspace.
    /// </summary>
    Task StartAsync(CancellationToken ct = default);

    /// <summary>
    /// Invoked when the plugin is disabled or prior to unloading.
    /// </summary>
    Task StopAsync(CancellationToken ct = default);
}


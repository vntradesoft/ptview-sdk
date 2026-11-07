using System.Text.Json.Serialization;

namespace PTView.SDK.Plugins;

/// <summary>
/// Plugin manifest loaded from plugin.json defining identity, version, and compatibility constraints.
/// </summary>
public sealed record PluginManifest
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    [JsonPropertyName("minSdkVersion")]
    public string MinSdkVersion { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("author")]
    public string Author { get; init; } = string.Empty;

    [JsonPropertyName("entryPointDll")]
    public string EntryPointDll { get; init; } = string.Empty;

    [JsonPropertyName("entryPointClass")]
    public string? EntryPointClass { get; init; }

    [JsonPropertyName("dependencies")]
    public IReadOnlyList<string> Dependencies { get; init; } = [];

    /// <summary>
    /// Validates the manifest against SDK version and structural rules.
    /// </summary>
    public (bool IsValid, IReadOnlyList<string> Errors) Validate(Version currentSdkVersion)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Id))
            errors.Add("Plugin 'id' is required.");

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Plugin 'name' is required.");

        if (string.IsNullOrWhiteSpace(Version) || !System.Version.TryParse(Version, out _))
            errors.Add($"Plugin 'version' ('{Version}') is invalid or not in semantic version format.");

        if (string.IsNullOrWhiteSpace(EntryPointDll))
            errors.Add("Plugin 'entryPointDll' is required.");

        if (!string.IsNullOrWhiteSpace(MinSdkVersion))
        {
            if (!System.Version.TryParse(MinSdkVersion, out var parsedMinSdk))
            {
                errors.Add($"Plugin 'minSdkVersion' ('{MinSdkVersion}') is not a valid version string.");
            }
            else if (currentSdkVersion < parsedMinSdk)
            {
                errors.Add($"Plugin requires SDK version {MinSdkVersion} but current runtime is {currentSdkVersion}.");
            }
        }

        return (errors.Count == 0, errors);
    }
}


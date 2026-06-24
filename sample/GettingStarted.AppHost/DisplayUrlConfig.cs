namespace GettingStarted.AppHost;

/// <summary>
/// Configuration for a custom URL shown in the Aspire dashboard.
/// </summary>
/// <param name="EndpointName">The endpoint to base the URL on (e.g. "https").</param>
/// <param name="DisplayText">The text shown for the URL in the dashboard.</param>
/// <param name="Path">Optional path appended to the endpoint URL.</param>
internal sealed record DisplayUrlConfig(
    string EndpointName,
    string DisplayText,
    string? Path = null);
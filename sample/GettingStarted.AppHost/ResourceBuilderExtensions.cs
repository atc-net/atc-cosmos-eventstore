namespace GettingStarted.AppHost;

internal static class ResourceBuilderExtensions
{
    /// <summary>
    /// Moves all dashboard URLs into the details panel, except those whose display text is listed.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <param name="builder">The resource builder.</param>
    /// <param name="keepVisible">Display texts of URLs to keep on the resource card.</param>
    /// <returns>The resource builder for chaining.</returns>
    public static IResourceBuilder<T> WithUrlsHidden<T>(
        this IResourceBuilder<T> builder,
        params string[] keepVisible)
        where T : IResourceWithEndpoints
        => builder.WithUrls(context =>
        {
            var hidden = context.Urls
                .Where(url => !keepVisible.Contains(url.DisplayText, StringComparer.OrdinalIgnoreCase));

            foreach (var url in hidden)
            {
                url.DisplayLocation = UrlDisplayLocation.DetailsOnly;
            }
        });

    /// <summary>
    /// Adds a Scalar API reference URL to the dashboard and hides the default framework URLs.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <param name="builder">The resource builder.</param>
    /// <returns>The resource builder for chaining.</returns>
    public static IResourceBuilder<T> WithScalarUrl<T>(
        this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
        => builder.WithDisplayUrls(new DisplayUrlConfig("https", "Scalar", "scalar/v1"));

    private static IResourceBuilder<T> WithDisplayUrls<T>(
        this IResourceBuilder<T> builder,
        params DisplayUrlConfig[] urls)
        where T : IResourceWithEndpoints
        => builder
            .WithUrlsHidden()
            .WithUrls(context =>
            {
                foreach (var config in urls)
                {
                    var endpoint = context.GetEndpoint(config.EndpointName);
                    if (endpoint is null)
                    {
                        continue;
                    }

                    var url = string.IsNullOrEmpty(config.Path)
                        ? endpoint.Url
                        : $"{endpoint.Url}/{config.Path.TrimStart('/')}";

                    context.Urls.Add(new ResourceUrlAnnotation
                    {
                        Url = url,
                        DisplayText = config.DisplayText,
                        Endpoint = endpoint,
                    });
                }
            });
}
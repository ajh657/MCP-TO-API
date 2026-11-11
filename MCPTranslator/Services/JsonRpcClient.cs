using System.Net.Http.Json;
using System.Text.Json;
using MCPTranslator.Models;

namespace MCPTranslator.Services;

public sealed class JsonRpcClient(HttpClient httpClient) : IJsonRpcClient
{
    public async Task<JsonDocument> SendAsync(
        Uri endpoint,
        JsonRpcRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        ArgumentNullException.ThrowIfNull(request);

        if (!endpoint.IsAbsoluteUri ||
            (endpoint.Scheme != Uri.UriSchemeHttp && endpoint.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("The endpoint must be an absolute HTTP or HTTPS URI.", nameof(endpoint));
        }

        using var response = await httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);
    }
}

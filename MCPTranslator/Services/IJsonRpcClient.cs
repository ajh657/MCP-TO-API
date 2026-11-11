using System.Text.Json;
using MCPTranslator.Models;

namespace MCPTranslator.Services;

public interface IJsonRpcClient
{
    Task<JsonDocument> SendAsync(
        Uri endpoint,
        JsonRpcRequestDto request,
        CancellationToken cancellationToken = default);
}

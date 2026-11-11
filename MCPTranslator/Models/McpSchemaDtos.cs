using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCPTranslator.Models;

public static class McpProtocol
{
    public const string LatestVersion = "2026-07-28";
    public const string JsonRpcVersion = "2.0";
    public const int ParseError = -32700;
    public const int InvalidRequest = -32600;
    public const int MethodNotFound = -32601;
    public const int InvalidParams = -32602;
    public const int InternalError = -32603;
    public const int HeaderMismatch = -32020;
    public const int MissingRequiredClientCapability = -32021;
    public const int UnsupportedProtocolVersion = -32022;
}

public abstract class McpDto
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class JsonRpcRequestDto : McpDto
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = McpProtocol.JsonRpcVersion;

    [JsonPropertyName("id")]
    public JsonElement Id { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;
}

public class JsonRpcNotificationDto : McpDto
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = McpProtocol.JsonRpcVersion;

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;
}

public class JsonRpcRequestMessageDto : JsonRpcRequestDto
{
    [JsonPropertyName("params")]
    public JsonElement? Params { get; set; }
}

public class JsonRpcNotificationMessageDto : JsonRpcNotificationDto
{
    [JsonPropertyName("params")]
    public JsonElement? Params { get; set; }
}

public class JsonRpcResultResponseDto : McpDto
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = McpProtocol.JsonRpcVersion;

    [JsonPropertyName("id")]
    public JsonElement Id { get; set; }

    [JsonPropertyName("result")]
    public ResultDto Result { get; set; } = new();
}

public class JsonRpcErrorResponseDto : McpDto
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = McpProtocol.JsonRpcVersion;

    [JsonPropertyName("id")]
    public JsonElement? Id { get; set; }

    [JsonPropertyName("error")]
    public ErrorDto Error { get; set; } = new();
}

public class ErrorDto : McpDto
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public JsonElement? Data { get; set; }
}

public sealed class ParseErrorDto : ErrorDto { public ParseErrorDto() => Code = McpProtocol.ParseError; }
public sealed class InvalidRequestErrorDto : ErrorDto { public InvalidRequestErrorDto() => Code = McpProtocol.InvalidRequest; }
public sealed class MethodNotFoundErrorDto : ErrorDto { public MethodNotFoundErrorDto() => Code = McpProtocol.MethodNotFound; }
public sealed class InvalidParamsErrorDto : ErrorDto { public InvalidParamsErrorDto() => Code = McpProtocol.InvalidParams; }
public sealed class InternalErrorDto : ErrorDto { public InternalErrorDto() => Code = McpProtocol.InternalError; }

public class HeaderMismatchErrorDto : JsonRpcErrorResponseDto
{
    public HeaderMismatchErrorDto() => Error.Code = McpProtocol.HeaderMismatch;
}

public class UnsupportedProtocolVersionErrorDataDto : McpDto
{
    [JsonPropertyName("supported")]
    public List<string> Supported { get; set; } = new();

    [JsonPropertyName("requested")]
    public string Requested { get; set; } = string.Empty;
}

public class UnsupportedProtocolVersionErrorDto : JsonRpcErrorResponseDto
{
    public UnsupportedProtocolVersionErrorDto() => Error.Code = McpProtocol.UnsupportedProtocolVersion;

    [JsonIgnore]
    public UnsupportedProtocolVersionErrorDataDto? Data
    {
        get => Error.Data is { } value ? value.Deserialize<UnsupportedProtocolVersionErrorDataDto>() : null;
        set => Error.Data = value is null ? null : JsonSerializer.SerializeToElement(value);
    }
}

public class MissingRequiredClientCapabilityErrorDataDto : McpDto
{
    [JsonPropertyName("requiredCapabilities")]
    public ClientCapabilitiesDto RequiredCapabilities { get; set; } = new();
}

public class MissingRequiredClientCapabilityErrorDto : JsonRpcErrorResponseDto
{
    public MissingRequiredClientCapabilityErrorDto() => Error.Code = McpProtocol.MissingRequiredClientCapability;

    [JsonIgnore]
    public MissingRequiredClientCapabilityErrorDataDto? Data
    {
        get => Error.Data is { } value ? value.Deserialize<MissingRequiredClientCapabilityErrorDataDto>() : null;
        set => Error.Data = value is null ? null : JsonSerializer.SerializeToElement(value);
    }
}

public class RequestParamsDto : McpDto
{
    [JsonPropertyName("_meta")]
    public RequestMetaObjectDto Meta { get; set; } = new();
}

public class NotificationParamsDto : McpDto
{
    [JsonPropertyName("_meta")]
    public NotificationMetaObjectDto? Meta { get; set; }
}

public class InputResponseRequestParamsDto : RequestParamsDto
{
    [JsonPropertyName("inputResponses")]
    public Dictionary<string, JsonElement>? InputResponses { get; set; }

    [JsonPropertyName("requestState")]
    public string? RequestState { get; set; }
}

public class PaginatedRequestParamsDto : RequestParamsDto
{
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}

public class RequestMetaObjectDto : McpDto
{
    [JsonPropertyName("progressToken")]
    public JsonElement? ProgressToken { get; set; }

    [JsonPropertyName("io.modelcontextprotocol/protocolVersion")]
    public string ProtocolVersion { get; set; } = McpProtocol.LatestVersion;

    [JsonPropertyName("io.modelcontextprotocol/clientInfo")]
    public ImplementationDto? ClientInfo { get; set; }

    [JsonPropertyName("io.modelcontextprotocol/clientCapabilities")]
    public ClientCapabilitiesDto ClientCapabilities { get; set; } = new();

    [JsonPropertyName("io.modelcontextprotocol/logLevel")]
    public string? LogLevel { get; set; }
}

public class NotificationMetaObjectDto : McpDto
{
    [JsonPropertyName("io.modelcontextprotocol/subscriptionId")]
    public JsonElement? SubscriptionId { get; set; }
}

public class ResultMetaObjectDto : McpDto
{
    [JsonPropertyName("io.modelcontextprotocol/serverInfo")]
    public ImplementationDto? ServerInfo { get; set; }
}

public class ResultDto : McpDto
{
    [JsonPropertyName("_meta")]
    public ResultMetaObjectDto? Meta { get; set; }

    [JsonPropertyName("resultType")]
    public string ResultType { get; set; } = "complete";
}

public class InputRequiredResultDto : ResultDto
{
    [JsonPropertyName("inputRequests")]
    public Dictionary<string, JsonElement>? InputRequests { get; set; }

    [JsonPropertyName("requestState")]
    public string? RequestState { get; set; }
}

public class ClientCapabilitiesDto : McpDto
{
    [JsonPropertyName("experimental")]
    public Dictionary<string, JsonElement>? Experimental { get; set; }

    [JsonPropertyName("roots")]
    public JsonElement? Roots { get; set; }

    [JsonPropertyName("sampling")]
    public SamplingCapabilitiesDto? Sampling { get; set; }

    [JsonPropertyName("elicitation")]
    public ElicitationCapabilitiesDto? Elicitation { get; set; }

    [JsonPropertyName("extensions")]
    public Dictionary<string, JsonElement>? Extensions { get; set; }
}

public class SamplingCapabilitiesDto : McpDto
{
    [JsonPropertyName("context")]
    public JsonElement? Context { get; set; }

    [JsonPropertyName("tools")]
    public JsonElement? Tools { get; set; }
}

public class ElicitationCapabilitiesDto : McpDto
{
    [JsonPropertyName("form")]
    public JsonElement? Form { get; set; }

    [JsonPropertyName("url")]
    public JsonElement? Url { get; set; }
}

public class ServerCapabilitiesDto : McpDto
{
    [JsonPropertyName("experimental")]
    public Dictionary<string, JsonElement>? Experimental { get; set; }

    [JsonPropertyName("logging")]
    public JsonElement? Logging { get; set; }

    [JsonPropertyName("completions")]
    public JsonElement? Completions { get; set; }

    [JsonPropertyName("prompts")]
    public PromptsCapabilityDto? Prompts { get; set; }

    [JsonPropertyName("resources")]
    public ResourcesCapabilityDto? Resources { get; set; }

    [JsonPropertyName("tools")]
    public ToolsCapabilityDto? Tools { get; set; }

    [JsonPropertyName("extensions")]
    public Dictionary<string, JsonElement>? Extensions { get; set; }
}

public class PromptsCapabilityDto : McpDto
{
    [JsonPropertyName("listChanged")]
    public bool? ListChanged { get; set; }
}

public class ResourcesCapabilityDto : McpDto
{
    [JsonPropertyName("subscribe")]
    public bool? Subscribe { get; set; }

    [JsonPropertyName("listChanged")]
    public bool? ListChanged { get; set; }
}

public class ToolsCapabilityDto : McpDto
{
    [JsonPropertyName("listChanged")]
    public bool? ListChanged { get; set; }
}

public class IconDto : McpDto
{
    [JsonPropertyName("src")]
    public string Src { get; set; } = string.Empty;

    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }

    [JsonPropertyName("sizes")]
    public List<string>? Sizes { get; set; }

    [JsonPropertyName("theme")]
    public string? Theme { get; set; }
}

public class BaseMetadataDto : McpDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

public class ImplementationDto : BaseMetadataDto
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("websiteUrl")]
    public string? WebsiteUrl { get; set; }

    [JsonPropertyName("icons")]
    public List<IconDto>? Icons { get; set; }
}

public class AnnotationsDto : McpDto
{
    [JsonPropertyName("audience")]
    public List<string>? Audience { get; set; }

    [JsonPropertyName("priority")]
    public double? Priority { get; set; }

    [JsonPropertyName("lastModified")]
    public string? LastModified { get; set; }
}

public class ProgressNotificationParamsDto : NotificationParamsDto
{
    [JsonPropertyName("progressToken")]
    public JsonElement ProgressToken { get; set; }

    [JsonPropertyName("progress")]
    public double Progress { get; set; }

    [JsonPropertyName("total")]
    public double? Total { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

public class CancelledNotificationParamsDto : NotificationParamsDto
{
    [JsonPropertyName("requestId")]
    public JsonElement RequestId { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

public class SubscriptionFilterDto : McpDto
{
    [JsonPropertyName("toolsListChanged")]
    public bool? ToolsListChanged { get; set; }

    [JsonPropertyName("promptsListChanged")]
    public bool? PromptsListChanged { get; set; }

    [JsonPropertyName("resourcesListChanged")]
    public bool? ResourcesListChanged { get; set; }

    [JsonPropertyName("resourceSubscriptions")]
    public List<string>? ResourceSubscriptions { get; set; }
}

public class ResourceDto : BaseMetadataDto
{
    [JsonPropertyName("icons")]
    public List<IconDto>? Icons { get; set; }

    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }

    [JsonPropertyName("annotations")]
    public AnnotationsDto? Annotations { get; set; }

    [JsonPropertyName("size")]
    public long? Size { get; set; }

    [JsonPropertyName("_meta")]
    public Dictionary<string, JsonElement>? Meta { get; set; }
}

public class ResourceTemplateDto : BaseMetadataDto
{
    [JsonPropertyName("icons")]
    public List<IconDto>? Icons { get; set; }

    [JsonPropertyName("uriTemplate")]
    public string UriTemplate { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }

    [JsonPropertyName("annotations")]
    public AnnotationsDto? Annotations { get; set; }

    [JsonPropertyName("_meta")]
    public Dictionary<string, JsonElement>? Meta { get; set; }
}

public class ResourceContentsDto : McpDto
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;

    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("blob")]
    public string? Blob { get; set; }

    [JsonPropertyName("_meta")]
    public Dictionary<string, JsonElement>? Meta { get; set; }
}

public class PromptDto : BaseMetadataDto
{
    [JsonPropertyName("icons")]
    public List<IconDto>? Icons { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("arguments")]
    public List<PromptArgumentDto>? Arguments { get; set; }

    [JsonPropertyName("_meta")]
    public Dictionary<string, JsonElement>? Meta { get; set; }
}

public class PromptArgumentDto : BaseMetadataDto
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("required")]
    public bool? Required { get; set; }
}

public class PromptMessageDto : McpDto
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public ContentBlockDto Content { get; set; } = new TextContentDto();
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextContentDto), "text")]
[JsonDerivedType(typeof(ImageContentDto), "image")]
[JsonDerivedType(typeof(AudioContentDto), "audio")]
[JsonDerivedType(typeof(ResourceLinkDto), "resource_link")]
[JsonDerivedType(typeof(EmbeddedResourceDto), "resource")]
[JsonDerivedType(typeof(ToolUseContentDto), "tool_use")]
[JsonDerivedType(typeof(ToolResultContentDto), "tool_result")]
public abstract class ContentBlockDto : McpDto
{
    [JsonPropertyName("annotations")]
    public AnnotationsDto? Annotations { get; set; }

    [JsonPropertyName("_meta")]
    public Dictionary<string, JsonElement>? Meta { get; set; }
}

public class TextContentDto : ContentBlockDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

public class ImageContentDto : ContentBlockDto
{
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;

    [JsonPropertyName("mimeType")]
    public string MimeType { get; set; } = string.Empty;
}

public class AudioContentDto : ContentBlockDto
{
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;

    [JsonPropertyName("mimeType")]
    public string MimeType { get; set; } = string.Empty;
}

public class ResourceLinkDto : ResourceDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "resource_link";
}

public class EmbeddedResourceDto : ContentBlockDto
{
    [JsonPropertyName("resource")]
    public ResourceContentsDto Resource { get; set; } = new();
}

public class ToolUseContentDto : ContentBlockDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("input")]
    public Dictionary<string, JsonElement> Input { get; set; } = new();
}

public class ToolResultContentDto : ContentBlockDto
{
    [JsonPropertyName("toolUseId")]
    public string ToolUseId { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public List<ContentBlockDto> Content { get; set; } = new();

    [JsonPropertyName("structuredContent")]
    public JsonElement? StructuredContent { get; set; }

    [JsonPropertyName("isError")]
    public bool? IsError { get; set; }
}

public class ToolAnnotationsDto : McpDto
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("readOnlyHint")]
    public bool? ReadOnlyHint { get; set; }

    [JsonPropertyName("destructiveHint")]
    public bool? DestructiveHint { get; set; }

    [JsonPropertyName("idempotentHint")]
    public bool? IdempotentHint { get; set; }

    [JsonPropertyName("openWorldHint")]
    public bool? OpenWorldHint { get; set; }
}

public class ToolDto : BaseMetadataDto
{
    [JsonPropertyName("icons")]
    public List<IconDto>? Icons { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("inputSchema")]
    public Dictionary<string, JsonElement> InputSchema { get; set; } = new();

    [JsonPropertyName("outputSchema")]
    public Dictionary<string, JsonElement>? OutputSchema { get; set; }

    [JsonPropertyName("annotations")]
    public ToolAnnotationsDto? Annotations { get; set; }

    [JsonPropertyName("_meta")]
    public Dictionary<string, JsonElement>? Meta { get; set; }
}

public class CallToolRequestParamsDto : InputResponseRequestParamsDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("arguments")]
    public Dictionary<string, JsonElement>? Arguments { get; set; }
}

public class CallToolResultDto : ResultDto
{
    [JsonPropertyName("content")]
    public List<ContentBlockDto> Content { get; set; } = new();

    [JsonPropertyName("structuredContent")]
    public JsonElement? StructuredContent { get; set; }

    [JsonPropertyName("isError")]
    public bool? IsError { get; set; }
}

public class ListResultDto<T> : ResultDto
{
    [JsonPropertyName("nextCursor")]
    public string? NextCursor { get; set; }

    [JsonPropertyName("ttlMs")]
    public double TtlMs { get; set; }

    [JsonPropertyName("cacheScope")]
    public string CacheScope { get; set; } = string.Empty;
}

public class ListResourcesResultDto : ListResultDto<ResourceDto>
{
    [JsonPropertyName("resources")]
    public List<ResourceDto> Resources { get; set; } = new();
}

public class ListResourceTemplatesResultDto : ListResultDto<ResourceTemplateDto>
{
    [JsonPropertyName("resourceTemplates")]
    public List<ResourceTemplateDto> ResourceTemplates { get; set; } = new();
}

public class ListPromptsResultDto : ListResultDto<PromptDto>
{
    [JsonPropertyName("prompts")]
    public List<PromptDto> Prompts { get; set; } = new();
}

public class ListToolsResultDto : ListResultDto<ToolDto>
{
    [JsonPropertyName("tools")]
    public List<ToolDto> Tools { get; set; } = new();
}

public class ReadResourceRequestParamsDto : InputResponseRequestParamsDto
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;
}

public class ReadResourceResultDto : ResultDto
{
    [JsonPropertyName("ttlMs")]
    public double TtlMs { get; set; }

    [JsonPropertyName("cacheScope")]
    public string CacheScope { get; set; } = string.Empty;

    [JsonPropertyName("contents")]
    public List<ResourceContentsDto> Contents { get; set; } = new();
}

public class GetPromptRequestParamsDto : InputResponseRequestParamsDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("arguments")]
    public Dictionary<string, string>? Arguments { get; set; }
}

public class GetPromptResultDto : ResultDto
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("messages")]
    public List<PromptMessageDto> Messages { get; set; } = new();
}

public class CompleteRequestParamsDto : RequestParamsDto
{
    [JsonPropertyName("ref")]
    public JsonElement Reference { get; set; }

    [JsonPropertyName("argument")]
    public CompletionArgumentDto Argument { get; set; } = new();

    [JsonPropertyName("context")]
    public CompletionContextDto? Context { get; set; }
}

public class CompletionArgumentDto : McpDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

public class CompletionContextDto : McpDto
{
    [JsonPropertyName("arguments")]
    public Dictionary<string, string>? Arguments { get; set; }
}

public class CompleteResultDto : ResultDto
{
    [JsonPropertyName("completion")]
    public CompletionDto Completion { get; set; } = new();
}

public class CompletionDto : McpDto
{
    [JsonPropertyName("values")]
    public List<string> Values { get; set; } = new();

    [JsonPropertyName("total")]
    public int? Total { get; set; }

    [JsonPropertyName("hasMore")]
    public bool? HasMore { get; set; }
}

public class ResourceTemplateReferenceDto : McpDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "ref/resource";

    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;
}

public class PromptReferenceDto : BaseMetadataDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "ref/prompt";
}

public class RootDto : McpDto
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("_meta")]
    public Dictionary<string, JsonElement>? Meta { get; set; }
}

public class ListRootsResultDto : McpDto
{
    [JsonPropertyName("roots")]
    public List<RootDto> Roots { get; set; } = new();
}

public class CreateMessageRequestParamsDto : McpDto
{
    [JsonPropertyName("messages")]
    public List<SamplingMessageDto> Messages { get; set; } = new();

    [JsonPropertyName("modelPreferences")]
    public ModelPreferencesDto? ModelPreferences { get; set; }

    [JsonPropertyName("systemPrompt")]
    public string? SystemPrompt { get; set; }

    [JsonPropertyName("includeContext")]
    public string? IncludeContext { get; set; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("maxTokens")]
    public int MaxTokens { get; set; }

    [JsonPropertyName("stopSequences")]
    public List<string>? StopSequences { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, JsonElement>? Metadata { get; set; }

    [JsonPropertyName("tools")]
    public List<ToolDto>? Tools { get; set; }

    [JsonPropertyName("toolChoice")]
    public ToolChoiceDto? ToolChoice { get; set; }
}

public class SamplingMessageDto : McpDto
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public JsonElement Content { get; set; }

    [JsonPropertyName("_meta")]
    public Dictionary<string, JsonElement>? Meta { get; set; }
}

public class ModelPreferencesDto : McpDto
{
    [JsonPropertyName("hints")]
    public List<ModelHintDto>? Hints { get; set; }

    [JsonPropertyName("costPriority")]
    public double? CostPriority { get; set; }

    [JsonPropertyName("speedPriority")]
    public double? SpeedPriority { get; set; }

    [JsonPropertyName("intelligencePriority")]
    public double? IntelligencePriority { get; set; }
}

public class ModelHintDto : McpDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class ToolChoiceDto : McpDto
{
    [JsonPropertyName("mode")]
    public string? Mode { get; set; }
}

public class CreateMessageResultDto : SamplingMessageDto
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("stopReason")]
    public string? StopReason { get; set; }
}

public class LoggingMessageNotificationParamsDto : NotificationParamsDto
{
    [JsonPropertyName("level")]
    public string Level { get; set; } = string.Empty;

    [JsonPropertyName("logger")]
    public string? Logger { get; set; }

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }
}

public class ElicitRequestFormParamsDto : McpDto
{
    [JsonPropertyName("mode")]
    public string? Mode { get; set; } = "form";

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("requestedSchema")]
    public FormSchemaDto RequestedSchema { get; set; } = new();
}

public class ElicitRequestUrlParamsDto : McpDto
{
    [JsonPropertyName("mode")]
    public string Mode { get; set; } = "url";

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public class FormSchemaDto : McpDto
{
    [JsonPropertyName("$schema")]
    public string? Schema { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public Dictionary<string, PrimitiveSchemaDefinitionDto> Properties { get; set; } = new();

    [JsonPropertyName("required")]
    public List<string>? Required { get; set; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(StringSchemaDto), "string")]
[JsonDerivedType(typeof(NumberSchemaDto), "number")]
[JsonDerivedType(typeof(NumberSchemaDto), "integer")]
[JsonDerivedType(typeof(BooleanSchemaDto), "boolean")]
public abstract class PrimitiveSchemaDefinitionDto : McpDto
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class StringSchemaDto : PrimitiveSchemaDefinitionDto
{
    [JsonPropertyName("minLength")]
    public int? MinLength { get; set; }

    [JsonPropertyName("maxLength")]
    public int? MaxLength { get; set; }

    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("default")]
    public string? Default { get; set; }
}

public class NumberSchemaDto : PrimitiveSchemaDefinitionDto
{
    [JsonPropertyName("minimum")]
    public double? Minimum { get; set; }

    [JsonPropertyName("maximum")]
    public double? Maximum { get; set; }

    [JsonPropertyName("default")]
    public double? Default { get; set; }
}

public class BooleanSchemaDto : PrimitiveSchemaDefinitionDto
{
    [JsonPropertyName("default")]
    public bool? Default { get; set; }
}

public class ElicitResultDto : McpDto
{
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public Dictionary<string, JsonElement>? Content { get; set; }
}

public class DiscoverResultDto : ResultDto
{
    [JsonPropertyName("ttlMs")]
    public double TtlMs { get; set; }

    [JsonPropertyName("cacheScope")]
    public string CacheScope { get; set; } = string.Empty;

    [JsonPropertyName("supportedVersions")]
    public List<string> SupportedVersions { get; set; } = new();

    [JsonPropertyName("capabilities")]
    public ServerCapabilitiesDto Capabilities { get; set; } = new();

    [JsonPropertyName("instructions")]
    public string? Instructions { get; set; }
}

public class SubscriptionsListenRequestParamsDto : RequestParamsDto
{
    [JsonPropertyName("notifications")]
    public SubscriptionFilterDto Notifications { get; set; } = new();
}

public class SubscriptionsListenResultMetaObjectDto : ResultMetaObjectDto
{
    [JsonPropertyName("io.modelcontextprotocol/subscriptionId")]
    public JsonElement SubscriptionId { get; set; }
}

public class SubscriptionsListenResultDto : ResultDto
{
    [JsonPropertyName("_meta")]
    public new SubscriptionsListenResultMetaObjectDto Meta { get; set; } = new();
}

public class SubscriptionsAcknowledgedNotificationParamsDto : NotificationParamsDto
{
    [JsonPropertyName("notifications")]
    public SubscriptionFilterDto Notifications { get; set; } = new();
}

public class ResourceUpdatedNotificationParamsDto : NotificationParamsDto
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;
}

public class InputRequestDto : McpDto
{
    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("params")]
    public JsonElement? Params { get; set; }
}

public class InputResponseDto : McpDto
{
    [JsonPropertyName("result")]
    public JsonElement Result { get; set; }
}

public class TypedJsonRpcResultResponseDto<TResult> : McpDto where TResult : ResultDto
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = McpProtocol.JsonRpcVersion;

    [JsonPropertyName("id")]
    public JsonElement Id { get; set; }

    [JsonPropertyName("result")]
    public TResult Result { get; set; } = Activator.CreateInstance<TResult>();
}

public class DiscoverRequestDto : JsonRpcRequestDto
{
    public DiscoverRequestDto() => Method = "server/discover";
    [JsonPropertyName("params")] public RequestParamsDto? TypedParams { get; set; }
}

public class ListResourcesRequestDto : JsonRpcRequestDto
{
    public ListResourcesRequestDto() => Method = "resources/list";
    [JsonPropertyName("params")] public PaginatedRequestParamsDto? TypedParams { get; set; }
}

public class ListResourceTemplatesRequestDto : JsonRpcRequestDto
{
    public ListResourceTemplatesRequestDto() => Method = "resources/templates/list";
    [JsonPropertyName("params")] public PaginatedRequestParamsDto? TypedParams { get; set; }
}

public class ReadResourceRequestDto : JsonRpcRequestDto
{
    public ReadResourceRequestDto() => Method = "resources/read";
    [JsonPropertyName("params")] public ReadResourceRequestParamsDto? TypedParams { get; set; }
}

public class ListPromptsRequestDto : JsonRpcRequestDto
{
    public ListPromptsRequestDto() => Method = "prompts/list";
    [JsonPropertyName("params")] public PaginatedRequestParamsDto? TypedParams { get; set; }
}

public class GetPromptRequestDto : JsonRpcRequestDto
{
    public GetPromptRequestDto() => Method = "prompts/get";
    [JsonPropertyName("params")] public GetPromptRequestParamsDto? TypedParams { get; set; }
}

public class ListToolsRequestDto : JsonRpcRequestDto
{
    public ListToolsRequestDto() => Method = "tools/list";
    [JsonPropertyName("params")] public PaginatedRequestParamsDto? TypedParams { get; set; }
}

public class CallToolRequestDto : JsonRpcRequestDto
{
    public CallToolRequestDto() => Method = "tools/call";
    [JsonPropertyName("params")] public CallToolRequestParamsDto? TypedParams { get; set; }
}

public class CompleteRequestDto : JsonRpcRequestDto
{
    public CompleteRequestDto() => Method = "completion/complete";
    [JsonPropertyName("params")] public CompleteRequestParamsDto? TypedParams { get; set; }
}

public class SubscriptionsListenRequestDto : JsonRpcRequestDto
{
    public SubscriptionsListenRequestDto() => Method = "subscriptions/listen";
    [JsonPropertyName("params")] public SubscriptionsListenRequestParamsDto? TypedParams { get; set; }
}

public class CancelledNotificationDto : JsonRpcNotificationDto
{
    public CancelledNotificationDto() => Method = "notifications/cancelled";
    [JsonPropertyName("params")] public CancelledNotificationParamsDto? TypedParams { get; set; }
}

public class ProgressNotificationDto : JsonRpcNotificationDto
{
    public ProgressNotificationDto() => Method = "notifications/progress";
    [JsonPropertyName("params")] public ProgressNotificationParamsDto? TypedParams { get; set; }
}

public class ResourceUpdatedNotificationDto : JsonRpcNotificationDto
{
    public ResourceUpdatedNotificationDto() => Method = "notifications/resources/updated";
    [JsonPropertyName("params")] public ResourceUpdatedNotificationParamsDto? TypedParams { get; set; }
}

public class SubscriptionsAcknowledgedNotificationDto : JsonRpcNotificationDto
{
    public SubscriptionsAcknowledgedNotificationDto() => Method = "notifications/subscriptions/acknowledged";
    [JsonPropertyName("params")] public SubscriptionsAcknowledgedNotificationParamsDto? TypedParams { get; set; }
}

public class ResourceListChangedNotificationDto : JsonRpcNotificationDto
{
    public ResourceListChangedNotificationDto() => Method = "notifications/resources/list_changed";
    [JsonPropertyName("params")] public NotificationParamsDto? TypedParams { get; set; }
}

public class PromptListChangedNotificationDto : JsonRpcNotificationDto
{
    public PromptListChangedNotificationDto() => Method = "notifications/prompts/list_changed";
    [JsonPropertyName("params")] public NotificationParamsDto? TypedParams { get; set; }
}

public class ToolListChangedNotificationDto : JsonRpcNotificationDto
{
    public ToolListChangedNotificationDto() => Method = "notifications/tools/list_changed";
    [JsonPropertyName("params")] public NotificationParamsDto? TypedParams { get; set; }
}

public class LoggingMessageNotificationDto : JsonRpcNotificationDto
{
    public LoggingMessageNotificationDto() => Method = "notifications/message";
    [JsonPropertyName("params")] public LoggingMessageNotificationParamsDto? TypedParams { get; set; }
}

public class ListRootsRequestDto : McpDto
{
    [JsonPropertyName("method")]
    public string Method { get; set; } = "roots/list";

    [JsonPropertyName("params")]
    public NotificationParamsDto? Params { get; set; }
}

public class CreateMessageRequestDto : McpDto
{
    [JsonPropertyName("method")]
    public string Method { get; set; } = "sampling/createMessage";

    [JsonPropertyName("params")]
    public CreateMessageRequestParamsDto Params { get; set; } = new();
}

public class ElicitRequestDto : McpDto
{
    [JsonPropertyName("method")]
    public string Method { get; set; } = "elicitation/create";

    [JsonPropertyName("params")]
    public JsonElement Params { get; set; }
}

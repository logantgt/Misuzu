using System.Net.Mime;

namespace Misuzu.Services;

public class WebhookEntry
{
    public string Url { get; set; } = string.Empty;
    public string JsonBody { get; set; } = "{}"; // default empty JSON object
    public WebhookPayloadType PayloadType { get; set; }
    public WebhookAction Trigger { get; set; }
}

public enum WebhookAction
{
    Added,
    Removed
}

public enum WebhookPayloadType
{
    JSON,
    MultiPart
}

public class AppSettings
{
    public List<WebhookEntry> WebhookEntries { get; set; } = new();
}
namespace ECommerce.Domain.Entities;

public class RequestResponseLog
{
    public string Id { get; set; } = string.Empty;

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public string TraceId { get; set; } = string.Empty;

    public string Method { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public string RequestBody { get; set; } = string.Empty;

    public string ResponseBody { get; set; } = string.Empty;

    public IDictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

    public IDictionary<string, string> ResponseHeaders { get; set; } = new Dictionary<string, string>();
}



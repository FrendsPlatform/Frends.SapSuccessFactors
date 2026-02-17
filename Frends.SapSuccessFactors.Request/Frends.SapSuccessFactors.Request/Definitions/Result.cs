using System.Collections.Generic;

namespace Frends.SapSuccessFactors.Request.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if the task completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// Response body (string or JToken depending on ResultFormat)
    /// </summary>
    /// <example>{ "d": { "results": [{ "userId": "user123", "username": "john.doe" }] } }</example>
    public dynamic Data { get; set; }

    /// <summary>
    /// HTTP status code
    /// </summary>
    /// <example>200</example>
    public int StatusCode { get; set; }

    /// <summary>
    /// Response headers
    /// </summary>
    /// <example>{ "Content-Type": "application/json", "X-Request-ID": "abc123" }</example>
    public Dictionary<string, string> Headers { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}
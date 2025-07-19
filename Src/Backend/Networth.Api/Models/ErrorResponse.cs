namespace Networth.Models;

/// <summary>
/// Represents a standardized error response for API exceptions.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the HTTP status code of the error.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets a brief, human-readable summary of the error type.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a detailed explanation of the error.
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the request that caused this error.
    /// </summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the error occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the stack trace of the exception.
    /// </summary>
    public string? StackTrace { get; set; }
}

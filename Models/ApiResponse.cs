namespace Ticket_API.Models;

/// <summary>Item error terstruktur, dipakai untuk validasi maupun error bisnis.</summary>
public sealed record ApiError(
    string Code,
    string Message,
    string? Field = null);

/// <summary>Envelope standar semua response API.</summary>
public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    IReadOnlyList<ApiError>? Errors,
    string TraceId,
    DateTimeOffset Timestamp)
{
    public static ApiResponse<T> Ok(T? data, string message, string traceId) =>
        new(true, message, data, null, traceId, DateTimeOffset.UtcNow);

    public static ApiResponse<T> Fail(
        string message,
        IReadOnlyList<ApiError>? errors,
        string traceId) =>
        new(false, message, default, errors, traceId, DateTimeOffset.UtcNow);
}

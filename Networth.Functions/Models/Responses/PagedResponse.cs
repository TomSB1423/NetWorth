namespace Networth.Functions.Models.Responses;

/// <summary>
///     A generic paginated response model.
/// </summary>
/// <typeparam name="T">The type of items in the response.</typeparam>
public record PagedResponse<T>
{
    /// <summary>
    ///     Gets the items for the current page.
    /// </summary>
    public required IEnumerable<T> Items { get; init; }

    /// <summary>
    ///     Gets the current page number (1-based).
    /// </summary>
    public required int Page { get; init; }

    /// <summary>
    ///     Gets the number of items per page.
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    ///     Gets the total number of items across all pages.
    /// </summary>
    public required int TotalCount { get; init; }

    /// <summary>
    ///     Gets the total number of pages.
    /// </summary>
    public required int TotalPages { get; init; }

    /// <summary>
    ///     Gets a value indicating whether there is a next page.
    /// </summary>
    public required bool HasNextPage { get; init; }

    /// <summary>
    ///     Gets a value indicating whether there is a previous page.
    /// </summary>
    public required bool HasPreviousPage { get; init; }
}

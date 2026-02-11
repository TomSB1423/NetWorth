namespace Networth.Application.Models;

/// <summary>
///     Represents a paginated result set.
/// </summary>
/// <typeparam name="T">The type of items in the result set.</typeparam>
public record PagedResult<T>
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
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    ///     Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    ///     Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}

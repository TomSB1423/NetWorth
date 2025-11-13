using Networth.Domain.Entities;

namespace Networth.Domain.Repositories;

/// <summary>
///     Repository interface for institution metadata operations.
/// </summary>
public interface IInstitutionMetadataRepository
{
    /// <summary>
    ///     Gets all institutions for a specific country.
    /// </summary>
    /// <param name="countryCode">The country code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of institutions.</returns>
    Task<IEnumerable<InstitutionMetadata>> GetByCountryAsync(string countryCode, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Saves or updates institutions for a specific country.
    /// </summary>
    /// <param name="countryCode">The country code.</param>
    /// <param name="institutions">The institutions to save.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveInstitutionsAsync(string countryCode, IEnumerable<InstitutionMetadata> institutions, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes all institutions for a specific country.
    /// </summary>
    /// <param name="countryCode">The country code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteByCountryAsync(string countryCode, CancellationToken cancellationToken = default);
}

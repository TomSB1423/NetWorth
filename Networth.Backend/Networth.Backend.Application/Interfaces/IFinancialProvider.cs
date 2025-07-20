using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Application.Interfaces;

/// <summary>
/// Interface for financial provider services that handles all GoCardless operations.
/// </summary>
public interface IFinancialProvider
{
    /// <summary>
    /// Gets a list of available institutions for a given country.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of available institutions.</returns>
    Task<IEnumerable<Institution>> GetInstitutionsAsync(CancellationToken cancellationToken = default);
}

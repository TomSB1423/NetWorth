using System.Net.Http.Json;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Infrastructure.Gocardless;

/// <summary>
/// GoCardless implementation of financial provider services.
/// </summary>
internal class GocardlessService(IGocardlessClient gocardlessClient)
    : IFinancialProvider
{
    /// <inheritdoc />
    public async Task<IEnumerable<Institution>> GetInstitutionsAsync(CancellationToken cancellationToken = default)
    {
        var response = await gocardlessClient.GetInstitutions(cancellationToken);
        return response;
    }
}

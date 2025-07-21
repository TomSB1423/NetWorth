using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Infrastructure.Gocardless;

/// <summary>
/// GoCardless implementation of financial provider services.
/// </summary>
internal class GocardlessService(ILogger<GocardlessService> logger, IGocardlessClient gocardlessClient)
    : IFinancialProvider
{
    /// <inheritdoc />
    public async Task<IEnumerable<Institution>> GetInstitutionsAsync(CancellationToken cancellationToken = default)
    {
        var response = await gocardlessClient.GetInstitutions("GB", cancellationToken);

        var institutions = response.Select(dto =>
        {
            bool transactionParse = int.TryParse(dto.TransactionTotalDays, out var transactionTotalDays);
            bool maxAccessParse = int.TryParse(dto.MaxAccessValidForDays, out var maxAccessValidForDays);
            if (transactionParse || maxAccessParse)
            {
                logger.LogWarning("Failed to parse transaction days or access valid days for institution {InstitutionId}. Using default values.", dto.Id);
            }

            return new Institution
            {
                Id = dto.Id,
                Name = dto.Name,
                TransactionTotalDays = transactionTotalDays,
                MaxAccessValidForDays = maxAccessValidForDays,
                LogoUrl = dto.Logo,
            };
        });
        return institutions;
    }
}

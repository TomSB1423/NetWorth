using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;
using DomainInstitution = Networth.Domain.Entities.Institution;
using InfrastructureInstitutionMetadata = Networth.Infrastructure.Data.Entities.InstitutionMetadata;

namespace Networth.Infrastructure.Data.Repositories;

/// <summary>
///     Repository implementation for institution metadata operations.
/// </summary>
public class InstitutionMetadataRepository : IInstitutionMetadataRepository
{
    private readonly NetworthDbContext _context;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InstitutionMetadataRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public InstitutionMetadataRepository(NetworthDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DomainInstitution>> GetByCountryAsync(string countryCode, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Institutions
            .Where(i => i.CountryCode == countryCode)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomain);
    }

    /// <inheritdoc />
    public async Task SaveInstitutionsAsync(string countryCode, IEnumerable<DomainInstitution> institutions, CancellationToken cancellationToken = default)
    {
        // Delete existing institutions for this country
        await DeleteByCountryAsync(countryCode, cancellationToken);

        // Add new institutions
        var entities = institutions.Select(i => MapToInfrastructure(i, countryCode));
        await _context.Institutions.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteByCountryAsync(string countryCode, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Institutions
            .Where(i => i.CountryCode == countryCode)
            .ToListAsync(cancellationToken);

        _context.Institutions.RemoveRange(existing);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static DomainInstitution MapToDomain(InfrastructureInstitutionMetadata entity)
    {
        var countries = JsonSerializer.Deserialize<string[]>(entity.Countries) ?? [];

        return new DomainInstitution
        {
            Id = entity.Id,
            Name = entity.Name,
            TransactionTotalDays = entity.TransactionTotalDays,
            MaxAccessValidForDays = entity.MaxAccessValidForDays,
            LogoUrl = entity.LogoUrl,
            Bic = entity.Bic,
            Countries = countries,
        };
    }

    private static InfrastructureInstitutionMetadata MapToInfrastructure(DomainInstitution domain, string countryCode)
    {
        var countriesJson = JsonSerializer.Serialize(domain.Countries);

        return new InfrastructureInstitutionMetadata
        {
            Id = domain.Id,
            Name = domain.Name,
            TransactionTotalDays = domain.TransactionTotalDays,
            MaxAccessValidForDays = domain.MaxAccessValidForDays,
            LogoUrl = domain.LogoUrl,
            Bic = domain.Bic,
            CountryCode = countryCode,
            Countries = countriesJson,
            LastUpdated = DateTime.UtcNow,
        };
    }
}

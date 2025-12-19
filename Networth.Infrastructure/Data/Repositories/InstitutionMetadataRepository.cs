using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;
using DomainInstitutionMetadata = Networth.Domain.Entities.InstitutionMetadata;
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
    public async Task<IEnumerable<DomainInstitutionMetadata>> GetByCountryAsync(string countryCode, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Institutions
            .Where(i => i.CountryCode == countryCode)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomain);
    }

    /// <inheritdoc />
    public async Task SaveInstitutionsAsync(string countryCode, IEnumerable<DomainInstitutionMetadata> institutions, CancellationToken cancellationToken = default)
    {
        var incoming = institutions.ToList();

        var existingEntities = await _context.Institutions
            .Where(i => i.CountryCode == countryCode)
            .ToDictionaryAsync(i => i.Id, cancellationToken);

        foreach (var domainInst in incoming)
        {
            if (existingEntities.TryGetValue(domainInst.Id, out var entity))
            {
                // Update existing
                entity.Name = domainInst.Name;
                entity.LogoUrl = domainInst.LogoUrl;
                entity.Bic = domainInst.Bic;
                entity.Countries = JsonSerializer.Serialize(domainInst.Countries);
                entity.LastUpdated = DateTime.UtcNow;

                // Remove from dictionary so we know it was processed
                existingEntities.Remove(domainInst.Id);
            }
            else
            {
                // Insert new
                var newEntity = MapToInfrastructure(domainInst, countryCode);
                await _context.Institutions.AddAsync(newEntity, cancellationToken);
            }
        }

        // Delete any remaining entities that were not in the incoming list
        if (existingEntities.Count > 0)
        {
            _context.Institutions.RemoveRange(existingEntities.Values);
        }

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

    private static DomainInstitutionMetadata MapToDomain(InfrastructureInstitutionMetadata entity)
    {
        var countries = JsonSerializer.Deserialize<string[]>(entity.Countries) ?? [];

        return new DomainInstitutionMetadata
        {
            Id = entity.Id,
            Name = entity.Name,
            TransactionTotalDays = null,
            MaxAccessValidForDays = null,
            LogoUrl = entity.LogoUrl,
            Bic = entity.Bic,
            Countries = countries,
        };
    }

    private static InfrastructureInstitutionMetadata MapToInfrastructure(DomainInstitutionMetadata domain, string countryCode)
    {
        var countriesJson = JsonSerializer.Serialize(domain.Countries);

        return new InfrastructureInstitutionMetadata
        {
            Id = domain.Id,
            Name = domain.Name,
            LogoUrl = domain.LogoUrl,
            Bic = domain.Bic,
            CountryCode = countryCode,
            Countries = countriesJson,
            LastUpdated = DateTime.UtcNow,
        };
    }
}

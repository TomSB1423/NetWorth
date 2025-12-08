using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Repositories;
using Networth.ServiceDefaults;
using Xunit.Abstractions;
using DomainTransaction = Networth.Domain.Entities.Transaction;

namespace Networth.Functions.Tests.Integration;

public class TransactionRepositoryTests(MockoonTestFixture mockoonTestFixture, ITestOutputHelper testOutput)
    : IntegrationTestBase(mockoonTestFixture, testOutput)
{
    [Fact]
    public async Task UpsertTransactionsAsync_PersistsRunningBalance()
    {
        // Arrange
        var dbConnectionString = await App.GetConnectionStringAsync(ResourceNames.NetworthDb);

        var services = new ServiceCollection();
        services.AddDbContext<NetworthDbContext>(options => options.UseNpgsql(dbConnectionString));
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        var repository = new TransactionRepository(dbContext, NullLogger<TransactionRepository>.Instance);

        var userId = "test-user";
        var accountId = "test-account";

        // Seed parent entities to satisfy foreign keys
        dbContext.Users.Add(new Networth.Infrastructure.Data.Entities.User { Id = userId, Name = "Test User" });
        dbContext.Accounts.Add(new Networth.Infrastructure.Data.Entities.Account
        {
            Id = accountId,
            UserId = userId,
            InstitutionId = "inst",
            Iban = "IBAN",
            Name = "Acc",
            RequisitionId = "req",
            Currency = "EUR"
        });

        dbContext.Institutions.Add(new Networth.Infrastructure.Data.Entities.InstitutionMetadata
        {
            Id = "inst",
            Name = "Bank",
            CountryCode = "US",
            Countries = "[]",
            LastUpdated = DateTime.UtcNow
        });

        dbContext.Agreements.Add(new Networth.Infrastructure.Data.Entities.Agreement
        {
            Id = "agree",
            UserId = userId,
            InstitutionId = "inst",
            Created = DateTime.UtcNow,
            AccessScope = "[]"
        });

        dbContext.Requisitions.Add(new Networth.Infrastructure.Data.Entities.Requisition
        {
            Id = "req",
            UserId = userId,
            InstitutionId = "inst",
            AgreementId = "agree",
            Created = DateTime.UtcNow,
            Status = Domain.Enums.AccountLinkStatus.Linked,
            Reference = "ref",
            Link = "http://link"
        });

        await dbContext.SaveChangesAsync();

        var domainTransaction = new DomainTransaction
        {
            Id = "tx-1",
            AccountId = accountId,
            Amount = 100,
            Currency = "EUR",
            BalanceAfterTransaction = 500.50m, // This is the field we want to check
            BookingDate = DateTime.UtcNow,
            TransactionId = "bank-tx-1"
        };

        // Act
        await repository.UpsertTransactionsAsync(accountId, userId, [domainTransaction]);

        // Assert
        var savedTransaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == "tx-1");
        Assert.NotNull(savedTransaction);
        Assert.Equal(500.50m, savedTransaction.RunningBalance);
    }
}

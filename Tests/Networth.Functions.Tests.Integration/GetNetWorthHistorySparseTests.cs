using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Entities;
using Networth.Infrastructure.Data.Repositories;
using Networth.ServiceDefaults;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration;

public class GetNetWorthHistorySparseTests(MockoonTestFixture mockoonTestFixture, ITestOutputHelper testOutput)
    : IntegrationTestBase(mockoonTestFixture, testOutput)
{
    [Fact]
    public async Task GetNetWorthHistory_SkipsDaysWithNoChange()
    {
        var dbConnectionString = await App.GetConnectionStringAsync(ResourceNames.NetworthDb);
        if (string.IsNullOrEmpty(dbConnectionString))
        {
            Assert.Fail("Connection string not found");
        }

        // Setup DB Context
        var services = new ServiceCollection();
        services.AddDbContext<NetworthDbContext>(options => options.UseNpgsql(dbConnectionString));
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();

        await dbContext.Database.EnsureCreatedAsync();

        var userId = "sparse-user";
        var accountId = "sparse-acc";
        var institutionId = "inst-1";
        var agreementId = "agree-1";
        var requisitionId = "req-1";

        // Seed User, Institution, Agreement, Requisition, Account
        dbContext.Users.Add(new User { Id = userId, Name = "Sparse User" });

        dbContext.Institutions.Add(new InstitutionMetadata
        {
            Id = institutionId,
            Name = "Test Bank",
            LogoUrl = "logo.png",
            CountryCode = "DE",
            Countries = "[\"DE\"]",
            LastUpdated = DateTime.UtcNow
        });

        dbContext.Agreements.Add(new Agreement
        {
            Id = agreementId,
            UserId = userId,
            InstitutionId = institutionId,
            Created = DateTime.UtcNow,
            MaxHistoricalDays = 90,
            AccessValidForDays = 90,
            AccessScope = "[\"balances\", \"transactions\"]"
        });

        dbContext.Requisitions.Add(new Requisition
        {
            Id = requisitionId,
            UserId = userId,
            InstitutionId = institutionId,
            AgreementId = agreementId,
            Created = DateTime.UtcNow,
            Status = Domain.Enums.AccountLinkStatus.Linked,
            Link = "http://link",
            Reference = "ref"
        });

        dbContext.Accounts.Add(new Account
        {
            Id = accountId,
            UserId = userId,
            InstitutionId = institutionId,
            Iban = "IBAN1",
            Name = "Acc 1",
            RequisitionId = requisitionId,
            Currency = "EUR"
        });

        // Seed Transactions
        // Day 1: Balance 100
        dbContext.Transactions.Add(new Transaction
        {
            Id = "t1",
            UserId = userId,
            AccountId = accountId,
            Amount = 100,
            RunningBalance = 100,
            BookingDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            TransactionId = "t1-ext",
            Currency = "EUR"
        });

        // Day 3: Balance 150 (Change of +50)
        dbContext.Transactions.Add(new Transaction
        {
            Id = "t2",
            UserId = userId,
            AccountId = accountId,
            Amount = 50,
            RunningBalance = 150,
            BookingDate = new DateTime(2023, 1, 3, 0, 0, 0, DateTimeKind.Utc),
            TransactionId = "t2-ext",
            Currency = "EUR"
        });

        await dbContext.SaveChangesAsync();

        // Act
        var repository = new TransactionRepository(dbContext, NullLogger<TransactionRepository>.Instance);
        var history = (await repository.GetNetWorthHistoryAsync(userId)).ToList();

        // Assert
        Assert.Contains(history, x => x.Date == new DateTime(2023, 1, 1) && x.Amount == 100);
        Assert.Contains(history, x => x.Date == new DateTime(2023, 1, 3) && x.Amount == 150);

        // This assertion should FAIL before the fix
        Assert.DoesNotContain(history, x => x.Date == new DateTime(2023, 1, 2));
    }
}

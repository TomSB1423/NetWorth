using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Networth.Domain.Enums;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Entities;
using Networth.ServiceDefaults;
using Xunit.Abstractions;
using DomainEntities = Networth.Domain.Entities;

namespace Networth.Functions.Tests.Integration;

public class GetNetWorthHistoryTests(MockoonTestFixture mockoonTestFixture, ITestOutputHelper testOutput)
    : IClassFixture<MockoonTestFixture>
{
    [Fact]
    public async Task GetNetWorthHistory_ReturnsCorrectDailyTotals()
    {
        // Arrange
        await using var app = await DistributedApplicationTestFactory.CreateAsync(
            testOutput,
            mockoonTestFixture.MockoonBaseUrl);

        var dbConnectionString = await app.GetConnectionStringAsync(ResourceNames.NetworthDb);
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

        // Seed Data for "mock-user-123" (used by MockAuthenticationMiddleware)
        var userId = "mock-user-123";
        var accountId1 = "acc-1";
        var accountId2 = "acc-2";

        var user = await dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            user = new User { Id = userId, Name = "Mock User" };
            dbContext.Users.Add(user);
        }

        var agreement = new Agreement
        {
            Id = "agr-1",
            UserId = userId,
            InstitutionId = "inst-1",
            AccessScope = "[\"balances\", \"details\", \"transactions\"]",
            Created = DateTime.UtcNow,
            Reconfirmation = false
        };
        dbContext.Agreements.Add(agreement);

        var requisition = new Requisition
        {
            Id = "req-1",
            UserId = userId,
            InstitutionId = "inst-1",
            AgreementId = "agr-1",
            Reference = "ref-1",
            Status = AccountLinkStatus.Linked,
            Created = DateTime.UtcNow,
            Link = "https://example.com/auth"
        };
        dbContext.Requisitions.Add(requisition);

        var account1 = new Account
        {
            Id = accountId1,
            UserId = userId,
            Name = "Account 1",
            Currency = "EUR",
            Created = DateTime.UtcNow,
            InstitutionId = "inst-1",
            RequisitionId = "req-1",
            Status = AccountLinkStatus.Linked
        };
        var account2 = new Account
        {
            Id = accountId2,
            UserId = userId,
            Name = "Account 2",
            Currency = "EUR",
            Created = DateTime.UtcNow,
            InstitutionId = "inst-1",
            RequisitionId = "req-1",
            Status = AccountLinkStatus.Linked
        };
        await dbContext.Accounts.AddRangeAsync(account1, account2);

        // Dates
        var today = DateTime.UtcNow.Date;
        var day1 = today.AddDays(-2);
        var day2 = today.AddDays(-1);
        var day3 = today;

        // Transactions with RunningBalance pre-calculated
        // Account 1: Day 1 -> 100, Day 2 -> 150
        dbContext.Transactions.Add(new Transaction
        {
            Id = "tx-1-1",
            AccountId = accountId1,
            UserId = userId,
            TransactionId = "btx-1-1",
            Amount = 100,
            Currency = "EUR",
            BookingDate = day1,
            RunningBalance = 100,
            ImportedAt = day1
        });
        dbContext.Transactions.Add(new Transaction
        {
            Id = "tx-1-2",
            AccountId = accountId1,
            UserId = userId,
            TransactionId = "btx-1-2",
            Amount = 50,
            Currency = "EUR",
            BookingDate = day2,
            RunningBalance = 150,
            ImportedAt = day2
        });

        // Account 2: Day 1 -> 50, Day 3 -> 200
        dbContext.Transactions.Add(new Transaction
        {
            Id = "tx-2-1",
            AccountId = accountId2,
            UserId = userId,
            TransactionId = "btx-2-1",
            Amount = 50,
            Currency = "EUR",
            BookingDate = day1,
            RunningBalance = 50,
            ImportedAt = day1
        });
        dbContext.Transactions.Add(new Transaction
        {
            Id = "tx-2-2",
            AccountId = accountId2,
            UserId = userId,
            TransactionId = "btx-2-2",
            Amount = 150,
            Currency = "EUR",
            BookingDate = day3,
            RunningBalance = 200,
            ImportedAt = day3
        });

        await dbContext.SaveChangesAsync();

        // Act
        var httpClient = app.CreateHttpClient(ResourceNames.Functions);
        var response = await httpClient.GetAsync("api/statistics/net-worth");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var history = await response.Content.ReadFromJsonAsync<IEnumerable<DomainEntities.NetWorthPoint>>();
        Assert.NotNull(history);
        var historyList = history.ToList();

        // Expected:
        // Day 1: Acc1(100) + Acc2(50) = 150
        // Day 2: Acc1(150) + Acc2(50, carried over) = 200
        // Day 3: Acc1(150, carried over) + Acc2(200) = 350

        Assert.True(historyList.Count >= 3, "Should have at least 3 data points");

        var p1 = historyList.FirstOrDefault(p => p.Date.Date == day1);
        var p2 = historyList.FirstOrDefault(p => p.Date.Date == day2);
        var p3 = historyList.FirstOrDefault(p => p.Date.Date == day3);

        Assert.NotNull(p1);
        Assert.Equal(150, p1.Amount);

        Assert.NotNull(p2);
        Assert.Equal(200, p2.Amount);

        Assert.NotNull(p3);
        Assert.Equal(350, p3.Amount);
    }
}

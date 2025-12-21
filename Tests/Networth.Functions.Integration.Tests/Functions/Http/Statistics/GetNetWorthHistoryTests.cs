using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Networth.Domain.Entities;
using Networth.Domain.Enums;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Entities;
using Networth.Infrastructure.Data.Repositories;
using Networth.ServiceDefaults;
using Xunit.Abstractions;
using Account = Networth.Infrastructure.Data.Entities.Account;
using Agreement = Networth.Infrastructure.Data.Entities.Agreement;
using InstitutionMetadata = Networth.Infrastructure.Data.Entities.InstitutionMetadata;
using Requisition = Networth.Infrastructure.Data.Entities.Requisition;
using Transaction = Networth.Infrastructure.Data.Entities.Transaction;

namespace Networth.Functions.Tests.Integration.Functions.Http.Statistics;

public class GetNetWorthHistoryTests(MockoonTestFixture mockoonTestFixture, ITestOutputHelper testOutput)
    : IntegrationTestBase(mockoonTestFixture, testOutput)
{
    [Fact]
    public async Task GetNetWorthHistory_ReturnsCorrectDailyTotals()
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

        // Seed Data for Constants.MockUserId (used by MockAuthenticationMiddleware)
        Guid userId = Constants.MockUserId;
        var accountId1 = "acc-1";
        var accountId2 = "acc-2";

        var user = await dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            user = new User { Id = userId, Name = "Mock User", FirebaseUid = Constants.MockFirebaseUid };
            dbContext.Users.Add(user);
        }

        var agreement = new Agreement
        {
            Id = "agr-1",
            UserId = userId,
            InstitutionId = "inst-1",
            AccessScope = "[\"balances\", \"details\", \"transactions\"]",
            Created = DateTime.UtcNow,
            Reconfirmation = false,
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
            Link = "https://example.com/auth",
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
            Status = AccountLinkStatus.Linked,
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
            Status = AccountLinkStatus.Linked,
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
            ImportedAt = day1,
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
            ImportedAt = day2,
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
            ImportedAt = day1,
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
            ImportedAt = day3,
        });

        await dbContext.SaveChangesAsync();

        // Act
        var historyResponse = await Client.GetNetWorthHistoryAsync();

        // Assert
        Assert.NotNull(historyResponse);
        var historyList = historyResponse.DataPoints.ToList();
        Assert.Equal(Networth.Domain.Enums.NetWorthCalculationStatus.Calculated, historyResponse.Status);
        Assert.NotNull(historyResponse.LastCalculated);

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

    [Fact]
    public async Task GetNetWorthHistory_ReturnsCorrectDailyTotals_SingleAccount()
    {
        // Arrange
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

        // Seed Data
        Guid userId = Constants.MockUserId;
        var accountId = "acc-single";

        var user = await dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            user = new User { Id = userId, Name = "Mock User Single", FirebaseUid = Constants.MockFirebaseUid };
            dbContext.Users.Add(user);
        }

        var agreement = new Agreement
        {
            Id = "agr-single",
            UserId = userId,
            InstitutionId = "inst-1",
            AccessScope = "[\"balances\", \"details\", \"transactions\"]",
            Created = DateTime.UtcNow,
            Reconfirmation = false,
        };
        dbContext.Agreements.Add(agreement);

        var requisition = new Requisition
        {
            Id = "req-single",
            UserId = userId,
            InstitutionId = "inst-1",
            AgreementId = "agr-single",
            Reference = "ref-single",
            Status = AccountLinkStatus.Linked,
            Created = DateTime.UtcNow,
            Link = "https://example.com/auth",
        };
        dbContext.Requisitions.Add(requisition);

        var account = new Account
        {
            Id = accountId,
            UserId = userId,
            Name = "Account Single",
            Currency = "EUR",
            Created = DateTime.UtcNow,
            InstitutionId = "inst-1",
            RequisitionId = "req-single",
            Status = AccountLinkStatus.Linked,
        };
        await dbContext.Accounts.AddAsync(account);

        // Dates
        var today = DateTime.UtcNow.Date;
        var day1 = today.AddDays(-2);
        var day2 = today.AddDays(-1);

        // Transactions
        // Day 1: 100
        dbContext.Transactions.Add(new Transaction
        {
            Id = "tx-s-1",
            AccountId = accountId,
            UserId = userId,
            TransactionId = "btx-s-1",
            Amount = 100,
            Currency = "EUR",
            BookingDate = day1,
            RunningBalance = 100,
            ImportedAt = day1,
        });
        // Day 2: 50 (Balance 150)
        dbContext.Transactions.Add(new Transaction
        {
            Id = "tx-s-2",
            AccountId = accountId,
            UserId = userId,
            TransactionId = "btx-s-2",
            Amount = 50,
            Currency = "EUR",
            BookingDate = day2,
            RunningBalance = 150,
            ImportedAt = day2,
        });

        await dbContext.SaveChangesAsync();

        var httpClient = App.CreateHttpClient(ResourceNames.Functions);

        // Act
        var response = await httpClient.GetAsync("api/statistics/net-worth");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var historyResponse = await response.Content.ReadFromJsonAsync<Networth.Functions.Models.Responses.NetWorthHistoryResponse>();
        Assert.NotNull(historyResponse);
        var historyList = historyResponse.DataPoints.ToList();
        Assert.Equal(Networth.Domain.Enums.NetWorthCalculationStatus.Calculated, historyResponse.Status);
        Assert.NotNull(historyResponse.LastCalculated);

        var p1 = historyList.FirstOrDefault(p => p.Date.Date == day1);
        var p2 = historyList.FirstOrDefault(p => p.Date.Date == day2);

        Assert.NotNull(p1);
        Assert.Equal(100, p1.Amount);

        Assert.NotNull(p2);
        Assert.Equal(150, p2.Amount);
    }

    [Fact]
    public async Task GetNetWorthHistory_SkipsDaysWithNoChange()
    {
        string? dbConnectionString = await App.GetConnectionStringAsync(ResourceNames.NetworthDb);
        if (string.IsNullOrEmpty(dbConnectionString))
        {
            Assert.Fail("Connection string not found");
        }

        // Setup DB Context
        ServiceCollection services = new();
        services.AddDbContext<NetworthDbContext>(options => options.UseNpgsql(dbConnectionString));
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        NetworthDbContext dbContext = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();

        await dbContext.Database.EnsureCreatedAsync();

        Guid userId = Guid.NewGuid();
        string accountId = "sparse-acc";
        string institutionId = "inst-1";
        string agreementId = "agree-1";
        string requisitionId = "req-1";

        // Seed User, Institution, Agreement, Requisition, Account
        dbContext.Users.Add(new User
        {
            Id = userId, Name = "Sparse User", FirebaseUid = "sparse-firebase-uid",
        });

        dbContext.Institutions.Add(new InstitutionMetadata
        {
            Id = institutionId,
            Name = "Test Bank",
            LogoUrl = "logo.png",
            CountryCode = "DE",
            Countries = "[\"DE\"]",
            LastUpdated = DateTime.UtcNow,
        });

        dbContext.Agreements.Add(new Agreement
        {
            Id = agreementId,
            UserId = userId,
            InstitutionId = institutionId,
            Created = DateTime.UtcNow,
            MaxHistoricalDays = 90,
            AccessValidForDays = 90,
            AccessScope = "[\"balances\", \"transactions\"]",
        });

        dbContext.Requisitions.Add(new Requisition
        {
            Id = requisitionId,
            UserId = userId,
            InstitutionId = institutionId,
            AgreementId = agreementId,
            Created = DateTime.UtcNow,
            Status = AccountLinkStatus.Linked,
            Link = "http://link",
            Reference = "ref",
        });

        dbContext.Accounts.Add(new Account
        {
            Id = accountId,
            UserId = userId,
            InstitutionId = institutionId,
            Iban = "IBAN1",
            Name = "Acc 1",
            RequisitionId = requisitionId,
            Currency = "EUR",
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
            Currency = "EUR",
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
            Currency = "EUR",
        });

        await dbContext.SaveChangesAsync();

        // Act
        TransactionRepository repository = new(dbContext, NullLogger<TransactionRepository>.Instance);
        List<NetWorthPoint> history = (await repository.GetNetWorthHistoryAsync(userId)).ToList();

        // Assert
        Assert.Contains(history, x => x.Date == new DateTime(2023, 1, 1) && x.Amount == 100);
        Assert.Contains(history, x => x.Date == new DateTime(2023, 1, 3) && x.Amount == 150);

        // This assertion should FAIL before the fix
        Assert.DoesNotContain(history, x => x.Date == new DateTime(2023, 1, 2));
    }
}

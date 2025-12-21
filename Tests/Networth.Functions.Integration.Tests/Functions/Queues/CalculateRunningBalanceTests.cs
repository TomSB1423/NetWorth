using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Networth.Application.Commands;
using Networth.Domain.Enums;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Entities;
using Networth.ServiceDefaults;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Functions.Queues;

public class CalculateRunningBalanceTests(MockoonTestFixture mockoonTestFixture, ITestOutputHelper testOutput)
    : IntegrationTestBase(mockoonTestFixture, testOutput)
{
    [Fact]
    public async Task CalculateRunningBalance_UpdatesTransactionsCorrectly()
    {
        // Get connection strings
        var dbConnectionString = await App.GetConnectionStringAsync(ResourceNames.NetworthDb);
        var storageConnectionString = await App.GetConnectionStringAsync(ResourceNames.Queues);

        if (string.IsNullOrEmpty(dbConnectionString) || string.IsNullOrEmpty(storageConnectionString))
        {
            Assert.Fail("Connection strings not found");
        }

        // Setup DB Context
        var services = new ServiceCollection();
        services.AddDbContext<NetworthDbContext>(options => options.UseNpgsql(dbConnectionString));
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();

        // Ensure DB is created
        await dbContext.Database.EnsureCreatedAsync();

        // Seed Data
        var accountId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId, Name = "Test User", FirebaseUid = "test-firebase-uid",
        };
        dbContext.Users.Add(user);

        var institution = new InstitutionMetadata
        {
            Id = "inst-1",
            Name = "Test Bank",
            CountryCode = "GB",
            Countries = "[\"GB\"]",
            LastUpdated = DateTime.UtcNow,
            LogoUrl = "logo.png",
        };

        var agreement = new Agreement
        {
            Id = "agree-1",
            InstitutionId = "inst-1",
            UserId = userId,
            MaxHistoricalDays = 90,
            AccessValidForDays = 90,
            AccessScope = "[\"balances\", \"details\", \"transactions\"]",
            Accepted = DateTime.UtcNow,
        };

        var requisition = new Requisition
        {
            Id = "req-1",
            InstitutionId = "inst-1",
            UserId = userId,
            Status = AccountLinkStatus.Linked,
            AgreementId = "agree-1",
            Link = "http://link",
            Reference = "ref",
        };

        var account = new Account
        {
            Id = accountId,
            UserId = userId,
            RequisitionId = "req-1",
            InstitutionId = "inst-1",
            Name = "Test Account",
            Currency = "EUR",
            Created = DateTime.UtcNow,
        };

        var balance = new AccountBalance
        {
            Id = Guid.NewGuid().ToString(),
            AccountId = accountId,
            BalanceType = "closingBooked",
            Amount = 1000m,
            Currency = "EUR",
            ReferenceDate = DateTime.UtcNow,
            RetrievedAt = DateTime.UtcNow,
        };

        var tx1 = new Transaction
        {
            Id = "tx-1",
            AccountId = accountId,
            UserId = userId,
            TransactionId = "bank-tx-1",
            Amount = -50m,
            Currency = "EUR",
            BookingDate = DateTime.UtcNow.AddHours(-1),
            ImportedAt = DateTime.UtcNow,
        };

        var tx2 = new Transaction
        {
            Id = "tx-2",
            AccountId = accountId,
            UserId = userId,
            TransactionId = "bank-tx-2",
            Amount = -20m,
            Currency = "EUR",
            BookingDate = DateTime.UtcNow.AddHours(-2),
            ImportedAt = DateTime.UtcNow,
        };

        dbContext.Institutions.Add(institution);
        dbContext.Agreements.Add(agreement);
        dbContext.Requisitions.Add(requisition);
        dbContext.Accounts.Add(account);
        dbContext.Set<AccountBalance>().Add(balance);
        dbContext.Set<Transaction>().Add(tx1);
        dbContext.Set<Transaction>().Add(tx2);
        await dbContext.SaveChangesAsync();

        // Act
        // Enqueue message
        var queueServiceClient = new QueueServiceClient(storageConnectionString);
        var queueClient = queueServiceClient.GetQueueClient(ResourceNames.CalculateRunningBalanceQueue);
        await queueClient.CreateIfNotExistsAsync();

        var command = new CalculateRunningBalanceCommand { AccountId = accountId };
        var messageJson = JsonSerializer.Serialize(command);
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        var base64Message = Convert.ToBase64String(messageBytes);

        await queueClient.SendMessageAsync(base64Message);

        // Assert
        // Poll DB
        var maxRetries = 30; // 30 seconds
        var delay = TimeSpan.FromSeconds(1);

        for (int i = 0; i < maxRetries; i++)
        {
            await Task.Delay(delay);

            // Reload transactions
            dbContext.ChangeTracker.Clear();
            var updatedTx1 = await dbContext.Set<Transaction>().FindAsync("tx-1");
            var updatedTx2 = await dbContext.Set<Transaction>().FindAsync("tx-2");

            if (updatedTx1?.RunningBalance != null && updatedTx2?.RunningBalance != null)
            {
                // Verify values
                // Latest balance is 1000.
                // tx1 is latest (1 hour ago). tx2 is older (2 hours ago).
                // Logic:
                // tx1.RunningBalance = 1000.
                // CurrentBalance becomes 1000 - (-50) = 1050.
                // tx2.RunningBalance = 1050.

                Assert.Equal(1000m, updatedTx1.RunningBalance);
                Assert.Equal(1050m, updatedTx2.RunningBalance);
                return;
            }
        }

        Assert.Fail("Running balances were not updated in time.");
    }
}

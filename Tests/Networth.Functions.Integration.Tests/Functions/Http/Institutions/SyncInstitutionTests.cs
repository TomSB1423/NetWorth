using Microsoft.EntityFrameworkCore;
using Networth.Domain.Enums;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Entities;
using Networth.ServiceDefaults;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Functions.Http.Institutions;

public class SyncInstitutionTests(MockoonTestFixture mockoonTestFixture, ITestOutputHelper testOutput)
    : IntegrationTestBase(mockoonTestFixture, testOutput)
{
    [Fact]
    public async Task SyncInstitution_EnqueuesSyncForLinkedAccounts()
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

        // Seed Data
        string userId = Constants.MockUserId;
        var institutionId = "inst-1";

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
            InstitutionId = institutionId,
            AccessScope = "[\"balances\", \"details\", \"transactions\"]",
            Created = DateTime.UtcNow,
            Reconfirmation = false,
        };
        dbContext.Agreements.Add(agreement);

        var requisition = new Requisition
        {
            Id = "req-1",
            UserId = userId,
            InstitutionId = institutionId,
            AgreementId = "agr-1",
            Reference = "ref-1",
            Status = AccountLinkStatus.Linked,
            Created = DateTime.UtcNow,
            Link = "https://example.com/auth",
        };
        dbContext.Requisitions.Add(requisition);

        var account1 = new Account
        {
            Id = "acc-1",
            UserId = userId,
            Name = "Account 1",
            Currency = "EUR",
            Created = DateTime.UtcNow,
            InstitutionId = institutionId,
            RequisitionId = "req-1",
            Status = AccountLinkStatus.Linked,
            LastSynced = null,
        };
        var account2 = new Account
        {
            Id = "acc-2",
            UserId = userId,
            Name = "Account 2",
            Currency = "EUR",
            Created = DateTime.UtcNow,
            InstitutionId = institutionId,
            RequisitionId = "req-1",
            Status = AccountLinkStatus.Linked,
            LastSynced = null,
        };
        await dbContext.Accounts.AddRangeAsync(account1, account2);
        await dbContext.SaveChangesAsync();

        await Client.SyncInstitutionAsync(institutionId);

        await AssertQueueMessageReceivedAsync(ResourceNames.InstitutionSyncQueue, "123");
    }

    [Fact]
    public async Task SyncInstitution_EnqueuesSyncForSingleAccount()
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
        string userId = Constants.MockUserId;
        var institutionId = "inst-1";

        var user = await dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            user = new User { Id = userId, Name = "Mock User" };
            dbContext.Users.Add(user);
        }

        var agreement = new Agreement
        {
            Id = "agr-single",
            UserId = userId,
            InstitutionId = institutionId,
            AccessScope = "[\"balances\", \"details\", \"transactions\"]",
            Created = DateTime.UtcNow,
            Reconfirmation = false,
        };
        dbContext.Agreements.Add(agreement);

        var requisition = new Requisition
        {
            Id = "req-single",
            UserId = userId,
            InstitutionId = institutionId,
            AgreementId = "agr-single",
            Reference = "ref-single",
            Status = AccountLinkStatus.Linked,
            Created = DateTime.UtcNow,
            Link = "https://example.com/auth",
        };
        dbContext.Requisitions.Add(requisition);

        var account = new Account
        {
            Id = "acc-single",
            UserId = userId,
            Name = "Account Single",
            Currency = "EUR",
            Created = DateTime.UtcNow,
            InstitutionId = institutionId,
            RequisitionId = "req-single",
            Status = AccountLinkStatus.Linked,
        };
        await dbContext.Accounts.AddAsync(account);
        await dbContext.SaveChangesAsync();

        await Client.SyncInstitutionAsync(institutionId);

        await AssertQueueMessageReceivedAsync(ResourceNames.InstitutionSyncQueue, "123");
    }

    [Fact]
    public async Task SyncInstitution_ReturnsZeroForNoAccounts()
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
        string userId = Constants.MockUserId;
        var institutionId = "inst-1";

        var user = await dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            user = new User { Id = userId, Name = "Mock User" };
            dbContext.Users.Add(user);
        }

        var agreement = new Agreement
        {
            Id = "agr-empty",
            UserId = userId,
            InstitutionId = institutionId,
            AccessScope = "[\"balances\", \"details\", \"transactions\"]",
            Created = DateTime.UtcNow,
            Reconfirmation = false,
        };
        dbContext.Agreements.Add(agreement);

        var requisition = new Requisition
        {
            Id = "req-empty",
            UserId = userId,
            InstitutionId = institutionId,
            AgreementId = "agr-empty",
            Reference = "ref-empty",
            Status = AccountLinkStatus.Linked,
            Created = DateTime.UtcNow,
            Link = "https://example.com/auth",
        };
        dbContext.Requisitions.Add(requisition);
        await dbContext.SaveChangesAsync();

        await Client.SyncInstitutionAsync(institutionId);

        await AssertQueueMessageReceivedAsync(ResourceNames.InstitutionSyncQueue, "123");
    }
}

using Microsoft.Extensions.Logging;
using Moq;
using Networth.Backend.Application.Commands;
using Networth.Backend.Application.Handlers;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Infrastructure.Tests.Unit.Handlers;

/// <summary>
///     Unit tests for LinkAccountCommandHandler.
/// </summary>
public class LinkAccountCommandHandlerTests
{
    private readonly Mock<IFinancialProvider> _mockFinancialProvider;
    private readonly Mock<ILogger<LinkAccountCommandHandler>> _mockLogger;
    private readonly LinkAccountCommandHandler _handler;

    public LinkAccountCommandHandlerTests()
    {
        _mockFinancialProvider = new Mock<IFinancialProvider>();
        _mockLogger = new Mock<ILogger<LinkAccountCommandHandler>>();
        _handler = new LinkAccountCommandHandler(_mockFinancialProvider.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_CreateAgreementAndRequisition_InCorrectOrder()
    {
        // Arrange
        const string institutionId = "test-institution";
        const string redirectUrl = "https://example.com/callback";
        const string reference = "test-ref-123";
        const string agreementId = "agreement-123";
        const string requisitionId = "requisition-456";

        var command = new LinkAccountCommand
        {
            InstitutionId = institutionId,
            RedirectUrl = redirectUrl,
            Reference = reference,
            MaxHistoricalDays = 90,
            AccessValidForDays = 90,
            UserLanguage = "EN",
        };

        var expectedAgreement = new Agreement
        {
            Id = agreementId,
            InstitutionId = institutionId,
            MaxHistoricalDays = 90,
            AccessValidForDays = 90,
            AccessScope = ["balances", "details", "transactions"],
            Created = DateTime.UtcNow,
            Accepted = null,
        };

        var expectedRequisition = new Requisition
        {
            Id = requisitionId,
            InstitutionId = institutionId,
            Agreement = agreementId,
            Reference = reference,
            Redirect = redirectUrl,
            Status = "CR", // Created
            UserLanguage = "EN",
            AuthorizationLink = "https://link.to.bank.com",
            Accounts = [],
            AccountSelection = false,
            RedirectImmediate = false,
        };

        _mockFinancialProvider
            .Setup(x => x.CreateAgreementAsync(institutionId, 90, 90, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAgreement);

        _mockFinancialProvider
            .Setup(x => x.CreateRequisitionAsync(redirectUrl, institutionId, agreementId, reference, "EN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRequisition);

        // Act
        LinkAccountCommandResult result = await _handler.HandleAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAgreement.Id, result.Agreement.Id);
        Assert.Equal(expectedRequisition.Id, result.Requisition.Id);
        Assert.Equal(agreementId, result.Requisition.Agreement);

        // Verify the methods were called in the correct order
        _mockFinancialProvider.Verify(
            x => x.CreateAgreementAsync(institutionId, 90, 90, It.IsAny<CancellationToken>()),
            Times.Once);

        _mockFinancialProvider.Verify(
            x => x.CreateRequisitionAsync(redirectUrl, institutionId, agreementId, reference, "EN", It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

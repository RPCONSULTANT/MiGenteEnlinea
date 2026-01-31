using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Infrastructure.Services;
using Xunit;

namespace MiGenteEnLinea.Infrastructure.Tests.Services;

/// <summary>
/// Unit Tests para MockPaymentService.
/// Valida que el servicio de pago mock funcione correctamente para desarrollo y testing.
/// </summary>
public class MockPaymentServiceTests
{
    private readonly Mock<ILogger<MockPaymentService>> _mockLogger;
    private readonly MockPaymentService _service;

    public MockPaymentServiceTests()
    {
        _mockLogger = new Mock<ILogger<MockPaymentService>>();
        _service = new MockPaymentService(_mockLogger.Object);
    }

    #region GenerateIdempotencyKeyAsync Tests

    /// <summary>
    /// Test 1: GenerateIdempotencyKeyAsync retorna un GUID válido
    /// </summary>
    [Fact]
    public async Task GenerateIdempotencyKeyAsync_ReturnsValidGuid()
    {
        // Act
        var result = await _service.GenerateIdempotencyKeyAsync();

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(result, out _).Should().BeTrue();
    }

    /// <summary>
    /// Test 2: GenerateIdempotencyKeyAsync retorna claves únicas
    /// </summary>
    [Fact]
    public async Task GenerateIdempotencyKeyAsync_ReturnsUniqueKeys()
    {
        // Act
        var key1 = await _service.GenerateIdempotencyKeyAsync();
        var key2 = await _service.GenerateIdempotencyKeyAsync();
        var key3 = await _service.GenerateIdempotencyKeyAsync();

        // Assert
        key1.Should().NotBe(key2);
        key2.Should().NotBe(key3);
        key1.Should().NotBe(key3);
    }

    #endregion

    #region ProcessPaymentAsync Tests

    /// <summary>
    /// Test 3: ProcessPaymentAsync siempre retorna Success=true (mock)
    /// </summary>
    [Fact]
    public async Task ProcessPaymentAsync_AlwaysReturnsSuccess()
    {
        // Arrange
        var request = new PaymentRequest
        {
            Amount = 1000.00m,
            ReferenceNumber = "REF-12345",
            CardNumber = "4111111111111111",
            ExpirationDate = "12/25",
            CVV = "123",
            ClientIP = "127.0.0.1"
        };

        // Act
        var result = await _service.ProcessPaymentAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.ResponseCode.Should().Be("00");
        result.ResponseDescription.Should().Contain("MOCK");
    }

    /// <summary>
    /// Test 4: ProcessPaymentAsync genera ApprovalCode con prefijo MOCK
    /// </summary>
    [Fact]
    public async Task ProcessPaymentAsync_GeneratesApprovalCodeWithMockPrefix()
    {
        // Arrange
        var request = CreateValidPaymentRequest();

        // Act
        var result = await _service.ProcessPaymentAsync(request);

        // Assert
        result.ApprovalCode.Should().StartWith("MOCK-");
    }

    /// <summary>
    /// Test 5: ProcessPaymentAsync genera TransactionReference con prefijo MOCK-TXN
    /// </summary>
    [Fact]
    public async Task ProcessPaymentAsync_GeneratesTransactionReferenceWithMockPrefix()
    {
        // Arrange
        var request = CreateValidPaymentRequest();

        // Act
        var result = await _service.ProcessPaymentAsync(request);

        // Assert
        result.TransactionReference.Should().StartWith("MOCK-TXN-");
    }

    /// <summary>
    /// Test 6: ProcessPaymentAsync genera IdempotencyKey válido
    /// </summary>
    [Fact]
    public async Task ProcessPaymentAsync_GeneratesValidIdempotencyKey()
    {
        // Arrange
        var request = CreateValidPaymentRequest();

        // Act
        var result = await _service.ProcessPaymentAsync(request);

        // Assert
        result.IdempotencyKey.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(result.IdempotencyKey, out _).Should().BeTrue();
    }

    /// <summary>
    /// Test 7: ProcessPaymentAsync acepta cualquier monto
    /// </summary>
    [Theory]
    [InlineData(0.01)]
    [InlineData(100.00)]
    [InlineData(9999.99)]
    [InlineData(1000000.00)]
    public async Task ProcessPaymentAsync_AcceptsAnyAmount(decimal amount)
    {
        // Arrange
        var request = new PaymentRequest
        {
            Amount = amount,
            ReferenceNumber = "TEST-" + amount,
            CardNumber = "4111111111111111",
            ExpirationDate = "12/25",
            CVV = "123",
            ClientIP = "127.0.0.1"
        };

        // Act
        var result = await _service.ProcessPaymentAsync(request);

        // Assert
        result.Success.Should().BeTrue();
    }

    #endregion

    #region GetConfigurationAsync Tests

    /// <summary>
    /// Test 8: GetConfigurationAsync retorna configuración mock válida
    /// </summary>
    [Fact]
    public async Task GetConfigurationAsync_ReturnsValidMockConfiguration()
    {
        // Act
        var config = await _service.GetConfigurationAsync();

        // Assert
        config.Should().NotBeNull();
        config.MerchantId.Should().Be("MOCK-MERCHANT-123");
        config.TerminalId.Should().Be("MOCK-TERM-456");
        config.BaseUrl.Should().Be("https://mock-gateway.test");
        config.IsTest.Should().BeTrue();
    }

    /// <summary>
    /// Test 9: GetConfigurationAsync siempre marca como IsTest=true
    /// </summary>
    [Fact]
    public async Task GetConfigurationAsync_AlwaysReturnsTestMode()
    {
        // Act
        var config = await _service.GetConfigurationAsync();

        // Assert
        config.IsTest.Should().BeTrue();
    }

    #endregion

    #region Constructor Tests

    /// <summary>
    /// Test 10: Constructor con logger válido no lanza excepción
    /// </summary>
    [Fact]
    public void Constructor_WithValidLogger_DoesNotThrow()
    {
        // Act
        Action act = () => new MockPaymentService(_mockLogger.Object);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Test 11: Servicio se instancia correctamente
    /// </summary>
    [Fact]
    public void Constructor_CreatesValidInstance()
    {
        // Act
        var service = new MockPaymentService(_mockLogger.Object);

        // Assert
        service.Should().NotBeNull();
    }

    #endregion

    #region Concurrent Calls Tests

    /// <summary>
    /// Test 12: Llamadas concurrentes generan claves únicas
    /// </summary>
    [Fact]
    public async Task ProcessPaymentAsync_ConcurrentCalls_GenerateUniqueReferences()
    {
        // Arrange
        var requests = Enumerable.Range(1, 10)
            .Select(i => new PaymentRequest
            {
                Amount = i * 100,
                ReferenceNumber = $"CONCURRENT-{i}",
                CardNumber = "4111111111111111",
                ExpirationDate = "12/25",
                CVV = "123",
                ClientIP = "127.0.0.1"
            })
            .ToList();

        // Act
        var tasks = requests.Select(r => _service.ProcessPaymentAsync(r));
        var results = await Task.WhenAll(tasks);

        // Assert
        var references = results.Select(r => r.TransactionReference).ToList();
        references.Should().OnlyHaveUniqueItems();
    }

    #endregion

    #region Helper Methods

    private static PaymentRequest CreateValidPaymentRequest()
    {
        return new PaymentRequest
        {
            Amount = 500.00m,
            ReferenceNumber = "REF-TEST-001",
            CardNumber = "4111111111111111",
            ExpirationDate = "12/25",
            CVV = "123",
            ClientIP = "127.0.0.1"
        };
    }

    #endregion
}

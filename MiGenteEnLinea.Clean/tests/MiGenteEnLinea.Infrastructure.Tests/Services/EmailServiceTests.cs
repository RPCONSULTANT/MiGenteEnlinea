using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MiGenteEnLinea.Infrastructure.Options;
using MiGenteEnLinea.Infrastructure.Services;
using Xunit;

namespace MiGenteEnLinea.Infrastructure.Tests.Services;

/// <summary>
/// Unit Tests para EmailService.
/// Estos tests validan la configuración y construcción del servicio,
/// sin hacer conexiones reales a servidores SMTP.
/// </summary>
public class EmailServiceTests
{
    private readonly Mock<ILogger<EmailService>> _mockLogger;

    public EmailServiceTests()
    {
        _mockLogger = new Mock<ILogger<EmailService>>();
    }

    #region EmailSettings Validation Tests

    /// <summary>
    /// Test 1: Validar que EmailSettings correctas no lanzan excepción
    /// </summary>
    [Fact]
    public void EmailSettings_ValidConfiguration_DoesNotThrowException()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            Username = "test@example.com",
            Password = "password",
            FromEmail = "from@example.com",
            FromName = "Test Sender",
            EnableSsl = true,
            Timeout = 30000,
            MaxRetryAttempts = 3,
            RetryDelayMilliseconds = 2000
        };

        // Act
        Action act = () => settings.Validate();

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Test 2: Validar que EmailSettings con SmtpServer vacío lanza excepción
    /// </summary>
    [Fact]
    public void EmailSettings_EmptySmtpServer_ThrowsInvalidOperationException()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "", // Inválido
            SmtpPort = 587,
            Username = "test@example.com",
            Password = "password",
            FromEmail = "from@example.com",
            FromName = "Test Sender"
        };

        // Act
        Action act = () => settings.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*SmtpServer*");
    }

    /// <summary>
    /// Test 3: Validar que EmailSettings con FromEmail vacío lanza excepción
    /// </summary>
    [Fact]
    public void EmailSettings_EmptyFromEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            Username = "test@example.com",
            Password = "password",
            FromEmail = "", // Vacío
            FromName = "Test Sender"
        };

        // Act
        Action act = () => settings.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*FromEmail*");
    }

    /// <summary>
    /// Test 4: Validar que EmailSettings con Username vacío lanza excepción
    /// </summary>
    [Fact]
    public void EmailSettings_EmptyUsername_ThrowsInvalidOperationException()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            Username = "", // Vacío
            Password = "password",
            FromEmail = "from@example.com",
            FromName = "Test Sender"
        };

        // Act
        Action act = () => settings.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Username*");
    }

    /// <summary>
    /// Test 5: Validar que EmailSettings con Password vacío lanza excepción
    /// </summary>
    [Fact]
    public void EmailSettings_EmptyPassword_ThrowsInvalidOperationException()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            Username = "test@example.com",
            Password = "", // Vacío
            FromEmail = "from@example.com",
            FromName = "Test Sender"
        };

        // Act
        Action act = () => settings.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Password*");
    }

    /// <summary>
    /// Test 6: Validar valores predeterminados de EmailSettings
    /// </summary>
    [Fact]
    public void EmailSettings_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var settings = new EmailSettings();

        // Assert
        settings.FromName.Should().Be("MiGente En Línea");
        settings.SmtpPort.Should().Be(587);
        settings.EnableSsl.Should().BeTrue();
        settings.MaxRetryAttempts.Should().Be(3);
        settings.RetryDelayMilliseconds.Should().Be(1000);
        settings.Timeout.Should().Be(30000);
    }

    #endregion

    #region EmailService Constructor Tests

    /// <summary>
    /// Test 7: Constructor con configuración válida no lanza excepción
    /// </summary>
    [Fact]
    public void Constructor_ValidEmailSettings_DoesNotThrowException()
    {
        // Arrange
        var validSettings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            Username = "test@example.com",
            Password = "password",
            FromEmail = "from@example.com",
            FromName = "Test Sender"
        };
        var options = Microsoft.Extensions.Options.Options.Create(validSettings);

        // Act
        Action act = () => new EmailService(options, _mockLogger.Object);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Test 8: Constructor con SmtpServer vacío lanza excepción
    /// </summary>
    [Fact]
    public void Constructor_EmptySmtpServer_ThrowsInvalidOperationException()
    {
        // Arrange
        var invalidSettings = new EmailSettings
        {
            SmtpServer = "", // Inválido
            SmtpPort = 587,
            Username = "test@example.com",
            Password = "password",
            FromEmail = "from@example.com"
        };
        var options = Microsoft.Extensions.Options.Options.Create(invalidSettings);

        // Act
        Action act = () => new EmailService(options, _mockLogger.Object);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*SmtpServer*");
    }

    /// <summary>
    /// Test 9: Constructor con FromEmail vacío lanza excepción
    /// </summary>
    [Fact]
    public void Constructor_EmptyFromEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var invalidSettings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            Username = "test@example.com",
            Password = "password",
            FromEmail = "" // Vacío
        };
        var options = Microsoft.Extensions.Options.Options.Create(invalidSettings);

        // Act
        Action act = () => new EmailService(options, _mockLogger.Object);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*FromEmail*");
    }

    /// <summary>
    /// Test 10: Constructor con Username vacío lanza excepción
    /// </summary>
    [Fact]
    public void Constructor_EmptyUsername_ThrowsInvalidOperationException()
    {
        // Arrange
        var invalidSettings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            Username = "", // Vacío
            Password = "password",
            FromEmail = "from@example.com"
        };
        var options = Microsoft.Extensions.Options.Options.Create(invalidSettings);

        // Act
        Action act = () => new EmailService(options, _mockLogger.Object);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Username*");
    }

    /// <summary>
    /// Test 11: Constructor con Password vacío lanza excepción
    /// </summary>
    [Fact]
    public void Constructor_EmptyPassword_ThrowsInvalidOperationException()
    {
        // Arrange
        var invalidSettings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            Username = "test@example.com",
            Password = "", // Vacío
            FromEmail = "from@example.com"
        };
        var options = Microsoft.Extensions.Options.Options.Create(invalidSettings);

        // Act
        Action act = () => new EmailService(options, _mockLogger.Object);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Password*");
    }

    #endregion

    #region EmailService Instance Tests

    /// <summary>
    /// Test 12: EmailService se instancia correctamente con configuración válida
    /// </summary>
    [Fact]
    public void EmailService_WithValidSettings_IsNotNull()
    {
        // Arrange
        var validSettings = CreateValidSettings();
        var options = Microsoft.Extensions.Options.Options.Create(validSettings);

        // Act
        var service = new EmailService(options, _mockLogger.Object);

        // Assert
        service.Should().NotBeNull();
    }

    /// <summary>
    /// Test 13: EmailSettings puede configurar valores personalizados
    /// </summary>
    [Fact]
    public void EmailSettings_CanSetCustomValues()
    {
        // Arrange & Act
        var settings = new EmailSettings
        {
            SmtpServer = "custom.smtp.server",
            SmtpPort = 2525,
            Username = "custom_user",
            Password = "custom_password",
            FromEmail = "custom@example.com",
            FromName = "Custom Sender",
            EnableSsl = false,
            MaxRetryAttempts = 5,
            RetryDelayMilliseconds = 5000,
            Timeout = 60000
        };

        // Assert
        settings.SmtpServer.Should().Be("custom.smtp.server");
        settings.SmtpPort.Should().Be(2525);
        settings.Username.Should().Be("custom_user");
        settings.Password.Should().Be("custom_password");
        settings.FromEmail.Should().Be("custom@example.com");
        settings.FromName.Should().Be("Custom Sender");
        settings.EnableSsl.Should().BeFalse();
        settings.MaxRetryAttempts.Should().Be(5);
        settings.RetryDelayMilliseconds.Should().Be(5000);
        settings.Timeout.Should().Be(60000);
    }

    /// <summary>
    /// Test 14: EmailSettings con null en SmtpServer lanza excepción
    /// </summary>
    [Fact]
    public void EmailSettings_NullSmtpServer_ThrowsInvalidOperationException()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = null!,
            SmtpPort = 587,
            Username = "test@example.com",
            Password = "password",
            FromEmail = "from@example.com"
        };

        // Act
        Action act = () => settings.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*SmtpServer*");
    }

    /// <summary>
    /// Test 15: EmailSettings con whitespace en campos lanza excepción
    /// </summary>
    [Fact]
    public void EmailSettings_WhitespaceInSmtpServer_ThrowsInvalidOperationException()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "   ", // Solo espacios
            SmtpPort = 587,
            Username = "test@example.com",
            Password = "password",
            FromEmail = "from@example.com"
        };

        // Act
        Action act = () => settings.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*SmtpServer*");
    }

    #endregion

    #region Helper Methods

    private static EmailSettings CreateValidSettings()
    {
        return new EmailSettings
        {
            SmtpServer = "smtp.test.com",
            SmtpPort = 587,
            Username = "test@test.com",
            Password = "test_password",
            FromEmail = "noreply@test.com",
            FromName = "Test Service",
            EnableSsl = true,
            Timeout = 30000,
            MaxRetryAttempts = 3,
            RetryDelayMilliseconds = 1000
        };
    }

    #endregion
}

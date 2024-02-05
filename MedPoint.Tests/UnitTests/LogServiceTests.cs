using MedPoint.Application.Services;
using MedPoint.Domain.Entities;
using MedPoint.Domain.Enums;
using MedPoint.Domain.Interfaces;

namespace MedPoint.Tests.UnitTests;

public class LogServiceTests
{
    private readonly Mock<ILogRepository> _mockLogRepository;
    private readonly LogService _logService;

    public LogServiceTests()
    {
        _mockLogRepository = new Mock<ILogRepository>();
        _logService = new LogService(_mockLogRepository.Object);
    }

    [Fact]
    public async Task LogActionAsync_CreatesLogEntrySuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        var action = LogAction.Create;
        var entity = "TestEntity";
        var details = "TestDetails";

        // No specific arrangement needed for the mock since we're verifying if the method is called

        // Act
        await _logService.LogActionAsync(action, userId, entity, entityId, details);

        // Assert
        _mockLogRepository.Verify(
            repo => repo.LogAsync(It.Is<Log>(log =>
                log.UserId == userId &&
                log.EntityId == entityId &&
                log.Action == action &&
                log.Entity == entity &&
                log.Details == details &&
                log.Timestamp != default(DateTime) // Ensures Timestamp is set
            )), Times.Once);
    }

    [Fact]
    public async Task LogActionAsync_WithMinimalDetails_LogsSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        var action = LogAction.Delete; // An action implying minimal detail might be needed
        var entity = "MinimalEntity";
        var details = string.Empty; // Minimal details

        // Act
        await _logService.LogActionAsync(action, userId, entity, entityId, details);

        // Assert
        _mockLogRepository.Verify(
            repo => repo.LogAsync(It.Is<Log>(log =>
                log.UserId == userId &&
                log.EntityId == entityId &&
                log.Action == action &&
                log.Entity == entity &&
                log.Details == details && // Verifies that empty details are handled
                log.Timestamp != default(DateTime)
            )), Times.Once);
    }

    [Fact]
    public async Task LogActionAsync_LogFailure_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        var action = LogAction.Update;
        var entity = "ErrorEntity";
        var details = "ErrorDetails";

        _mockLogRepository.Setup(repo => repo.LogAsync(It.IsAny<Log>()))
            .ThrowsAsync(new InvalidOperationException("Failed to log action"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _logService.LogActionAsync(action, userId, entity, entityId, details));
    }

}
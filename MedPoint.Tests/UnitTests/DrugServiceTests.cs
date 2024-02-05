using MedPoint.Application.Services;
using MedPoint.Domain.Entities;
using MedPoint.Domain.Enums;
using MedPoint.Domain.Interfaces;

namespace MedPoint.Tests.UnitTests;

public class DrugServiceTests
{
    private readonly Mock<IDrugRepository> _mockDrugRepository = new Mock<IDrugRepository>();
    private readonly Mock<IFDA_ApiService> _mockFDAService = new Mock<IFDA_ApiService>();
    private readonly Mock<ILogService> _mockLogService = new Mock<ILogService>();
    private readonly Mock<IUserService> _mockUserService = new Mock<IUserService>();
    private readonly DrugService _drugService;

    public DrugServiceTests()
    {
        _drugService = new DrugService(_mockDrugRepository.Object, _mockFDAService.Object, _mockLogService.Object, _mockUserService.Object);
    }

    [Fact]
    public async Task CreateDrugAsync_ValidInput_ReturnsDrug()
    {
        // Arrange
        var drug = new Drug { Id = Guid.NewGuid(), Name = "TestDrug", ChemicalName = "TestChemicalName" };
        var userId = Guid.NewGuid();
        _mockDrugRepository.Setup(repo => repo.CreateDrugAsync(It.IsAny<Drug>())).ReturnsAsync(drug);
        _mockDrugRepository.Setup(repo => repo.SearchDrugsAsync(It.IsAny<string>())).ReturnsAsync(new List<Drug>());
        _mockFDAService.Setup(service => service.ValidateDrugAsync(It.IsAny<Drug>())).Returns(Task.CompletedTask);
        _mockUserService.Setup(service => service.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new User { Id = userId, Role = "Admin" });

        // Act
        var result = await _drugService.CreateDrugAsync(drug, userId);

        // Assert
        Assert.Equal(drug.Id, result.Id);
        _mockLogService.Verify(log => log.LogActionAsync(LogAction.Create, userId, "Drug", drug.Id, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CreateDrugAsync_PermissionDenied_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var drug = new Drug { Name = "TestDrug", ChemicalName = "TestChemical" };
        var userId = Guid.NewGuid(); // Example user ID
        _mockUserService.Setup(service => service.GetUserByIdAsync(userId))
                        .ReturnsAsync(new User { Id = userId, Role = "User" }); // Role not authorized

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _drugService.CreateDrugAsync(drug, userId));
    }


    [Fact]
    public async Task CreateDrugAsync_ValidationFailure_ThrowsArgumentException()
    {
        // Arrange
        var drug = new Drug { Name = "", ChemicalName = "TestChemical" }; // Invalid input to trigger ArgumentException
        var userId = Guid.NewGuid();

        // Mock IUserService to return a valid user to bypass permission checks.
        _mockUserService.Setup(service => service.GetUserByIdAsync(userId))
                        .ReturnsAsync(new User { Id = userId, Role = "Admin" });

        // Setup the DrugRepository mock if needed, depending on the implementation details of CheckCreateUpdateRequirements
        _mockDrugRepository.Setup(repo => repo.SearchDrugsAsync(It.IsAny<string>()))
                           .ReturnsAsync(new List<Drug>()); // Ensure no duplicate check triggers

        // Act & Assert
        // Ensure ArgumentException is thrown due to invalid drug name
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _drugService.CreateDrugAsync(drug, userId));
        Assert.Contains("Name and ChemicalName are required fields", exception.Message);
    }


    [Fact]
    public async Task CreateDrugAsync_DuplicateDrug_ThrowsInvalidOperationException()
    {
        // Arrange
        var drug = new Drug { Name = "DuplicateDrug", ChemicalName = "DuplicateChemical" };
        var userId = Guid.NewGuid();

        // Mock IUserService to return a valid user with Admin role
        _mockUserService.Setup(service => service.GetUserByIdAsync(userId))
                        .ReturnsAsync(new User { Id = userId, Role = "Admin" });

        _mockDrugRepository.Setup(repo => repo.SearchDrugsAsync(drug.Name))
                           .ReturnsAsync(new List<Drug> { drug }); // Simulate existing drug

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _drugService.CreateDrugAsync(drug, userId));
    }

    [Fact]
    public async Task CreateDrugAsync_FDAValidationFails_ThrowsInvalidOperationException()
    {
        // Arrange
        var drug = new Drug { Name = "TestDrug", ChemicalName = "TestChemical" };
        var userId = Guid.NewGuid();
        _mockUserService.Setup(service => service.GetUserByIdAsync(userId))
                        .ReturnsAsync(new User { Id = userId, Role = "Admin" });
        _mockDrugRepository.Setup(repo => repo.SearchDrugsAsync(It.IsAny<string>()))
                           .ReturnsAsync(new List<Drug>()); // No duplicates
        _mockFDAService.Setup(service => service.ValidateDrugAsync(It.IsAny<Drug>()))
                       .ThrowsAsync(new InvalidOperationException("FDA validation failed."));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _drugService.CreateDrugAsync(drug, userId));
    }

}

using MedPoint.Application.Interfaces;
using MedPoint.Domain.Entities;
using MedPoint.Domain.Enums;
using MedPoint.Domain.Interfaces;

namespace MedPoint.Application.Services;

public class DrugService : IDrugService
{
    private readonly IDrugRepository _drugRepository;
    private readonly IFDA_ApiService _fdaApiService;
    private readonly ILogService _logService;
    private readonly IUserService _userService;
    public DrugService(IDrugRepository drugRepository, IFDA_ApiService fdaApiService, ILogService logService, IUserService userService)
    {
        _drugRepository = drugRepository;
        _fdaApiService = fdaApiService;
        _logService = logService;
        _userService = userService;
    }

    private async Task CheckCreateUpdateRequirements(Drug drug)
    {
        if (string.IsNullOrEmpty(drug.Name) || string.IsNullOrEmpty(drug.ChemicalName))
        {
            throw new ArgumentException("Name and ChemicalName are required fields.");
        }

        var existingDrugs = await _drugRepository.SearchDrugsAsync(drug.Name);
        if (existingDrugs.Any(d => d.ChemicalName == drug.ChemicalName) && drug.Id != existingDrugs.First().Id)
        {
            throw new InvalidOperationException("A drug with the same name and chemical name already exists.");
        }

        // Other business requirements could be implemented here...

        await _fdaApiService.ValidateDrugAsync(drug);
    }

    private async Task CheckPermissions(Guid userId, string action)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        if (!(user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || user.Role.Equals("Contributor", StringComparison.OrdinalIgnoreCase)))
        {
            throw new UnauthorizedAccessException($"User {userId} is not authorized to perform this action: {action}.");
        }
    }

    private async Task WriteLog(Guid userId, LogAction action, Guid entityId, string details = "")
    {
        await _logService.LogActionAsync(action, userId, "Drug", entityId, details);
    }

    public async Task<Drug> CreateDrugAsync(Drug drug, Guid userId)
    {
        await CheckPermissions(userId, "Create");
        await CheckCreateUpdateRequirements(drug);
        var createdDrug = await _drugRepository.CreateDrugAsync(drug);
        await WriteLog(userId, LogAction.Create, createdDrug.Id, drug.Name);
        return createdDrug;
    }

    public async Task UpdateDrugAsync(Drug drug, Guid userId)
    {
        await CheckPermissions(userId, "Update");
        await CheckCreateUpdateRequirements(drug);
        await _drugRepository.UpdateDrugAsync(drug);
        await WriteLog(userId, LogAction.Update, drug.Id, drug.Name);
    }

    public async Task DeleteDrugAsync(Guid id, Guid userId)
    {
        await CheckPermissions(userId, "Delete");
        var drug = await GetDrugByIdAsync(id);
        await _drugRepository.DeleteDrugAsync(id);
        await WriteLog(userId, LogAction.Delete, id, drug.Name);
    }

    public async Task<IEnumerable<Drug>> SearchDrugsAsync(string searchTerm)
    {
        return await _drugRepository.SearchDrugsAsync(searchTerm);
    }

    public async Task<IEnumerable<Drug>> ListDrugsAsync(int limit, int start)
    {
        return await _drugRepository.ListDrugsAsync(limit, start);
    }

    public async Task<Drug> GetDrugByIdAsync(Guid id)
    {
        return await _drugRepository.GetDrugByIdAsync(id);
    }
}
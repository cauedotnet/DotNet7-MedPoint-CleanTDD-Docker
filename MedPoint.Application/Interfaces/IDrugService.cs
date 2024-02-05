using MedPoint.Domain.Entities;

namespace MedPoint.Application.Interfaces;

public interface IDrugService
{
    Task<Drug> GetDrugByIdAsync(Guid id);
    Task<Drug> CreateDrugAsync(Drug drug, Guid userId);
    Task UpdateDrugAsync(Drug drug, Guid userId);
    Task DeleteDrugAsync(Guid id, Guid userId);
    Task<IEnumerable<Drug>> SearchDrugsAsync(string searchTerm);
    Task<IEnumerable<Drug>> ListDrugsAsync(int limit, int start);
}

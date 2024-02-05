using MedPoint.Domain.Entities;

namespace MedPoint.Domain.Interfaces;

public interface IDrugRepository
{
    Task<Drug> GetDrugByIdAsync(Guid id);
    Task<Drug> CreateDrugAsync(Drug drug);
    Task UpdateDrugAsync(Drug drug);
    Task DeleteDrugAsync(Guid id);
    Task<IEnumerable<Drug>> SearchDrugsAsync(string searchTerm);
    Task<IEnumerable<Drug>> ListDrugsAsync(int limit, int start);
}


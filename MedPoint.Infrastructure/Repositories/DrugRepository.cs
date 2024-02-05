using MongoDB.Bson;
using MongoDB.Driver;
using MedPoint.Domain.Entities;
using MedPoint.Domain.Interfaces;

namespace MedPoint.Infrastructure.Repositories;

public class DrugRepository : IDrugRepository
{
    private readonly MongoDbContext _context;

    public DrugRepository(MongoDbContext context)
    {
        _context = context;
    }

    //CRUD

    public async Task<Drug> GetDrugByIdAsync(Guid id)
    {
        return await _context.Drugs.Find(d => d.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Drug> CreateDrugAsync(Drug drug)
    {
        await _context.Drugs.InsertOneAsync(drug);
        return drug;
    }

    public async Task UpdateDrugAsync(Drug drug)
    {
        await _context.Drugs.ReplaceOneAsync(d => d.Id == drug.Id, drug);
    }

    public async Task DeleteDrugAsync(Guid id)
    {
        await _context.Drugs.DeleteOneAsync(d => d.Id == id);
    }

    //LIST & SEARCH

    public async Task<IEnumerable<Drug>> SearchDrugsAsync(string searchTerm)
    {
        var filter = Builders<Drug>.Filter.Or(
            Builders<Drug>.Filter.Regex("Name", new BsonRegularExpression(searchTerm, "i")),
            Builders<Drug>.Filter.Regex("ChemicalName", new BsonRegularExpression(searchTerm, "i")),
            Builders<Drug>.Filter.Regex("Manufacturer", new BsonRegularExpression(searchTerm, "i")),
            Builders<Drug>.Filter.Regex("Description", new BsonRegularExpression(searchTerm, "i")),
            Builders<Drug>.Filter.Regex("DosageAndAdministration", new BsonRegularExpression(searchTerm, "i"))
        );

        return await _context.Drugs.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Drug>> ListDrugsAsync(int limit, int start)
    {
        return await _context.Drugs.Find(_ => true).Skip(start).Limit(limit).ToListAsync();
    }
}
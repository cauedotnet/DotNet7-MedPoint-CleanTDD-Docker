using MongoDB.Driver;
using MedPoint.Domain.Entities;
using MedPoint.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Tests.IntegrationTests;

[Trait("Category", "IntegrationTests")]
public class DrugRepositoryTests : IDisposable
{
    private readonly MongoDbContext _context;
    private readonly DrugRepository _repository;

    public DrugRepositoryTests()
    {
        // Assuming MongoDbContext is your context that wraps around MongoDB collections
        _context = new MongoDbContext("mongodb://localhost:27017", "MedPoint_TestDB");
        _repository = new DrugRepository(_context);
    }

    [Fact]
    public async Task CreateDrugAsync_SavesToDatabase()
    {
        var drug = new Drug { Id = Guid.NewGuid(), Name = "TestDrug", ChemicalName = "TestChemical" };

        await _repository.CreateDrugAsync(drug);

        var savedDrug = await _context.Drugs.Find(x => x.Id == drug.Id).FirstOrDefaultAsync();
        Assert.NotNull(savedDrug);
        Assert.Equal("TestDrug", savedDrug.Name);

        // Cleanup
        await _context.Drugs.DeleteOneAsync(x => x.Id == drug.Id);
    }

    [Fact]
    public async Task GetDrugByIdAsync_ReturnsCorrectDrug()
    {
        var drug = new Drug { Id = Guid.NewGuid(), Name = "TestDrug", ChemicalName = "TestChemical" };
        await _context.Drugs.InsertOneAsync(drug);

        var fetchedDrug = await _repository.GetDrugByIdAsync(drug.Id);
        Assert.NotNull(fetchedDrug);
        Assert.Equal(drug.Name, fetchedDrug.Name);

        // Cleanup
        await _context.Drugs.DeleteOneAsync(x => x.Id == drug.Id);
    }

    [Fact]
    public async Task UpdateDrugAsync_UpdatesExistingDrug()
    {
        var drug = new Drug { Id = Guid.NewGuid(), Name = "OldName", ChemicalName = "OldChemicalName" };
        await _context.Drugs.InsertOneAsync(drug);

        drug.Name = "NewName";
        await _repository.UpdateDrugAsync(drug);

        var updatedDrug = await _context.Drugs.Find(x => x.Id == drug.Id).FirstOrDefaultAsync();
        Assert.NotNull(updatedDrug);
        Assert.Equal("NewName", updatedDrug.Name);

        // Cleanup
        await _context.Drugs.DeleteOneAsync(x => x.Id == drug.Id);
    }

    [Fact]
    public async Task DeleteDrugAsync_RemovesDrug()
    {
        var drug = new Drug { Id = Guid.NewGuid(), Name = "TestDrug", ChemicalName = "TestChemical" };
        await _context.Drugs.InsertOneAsync(drug);

        await _repository.DeleteDrugAsync(drug.Id);

        var deletedDrug = await _context.Drugs.Find(x => x.Id == drug.Id).FirstOrDefaultAsync();
        Assert.Null(deletedDrug);
    }

    [Fact]
    public async Task SearchDrugsAsync_ReturnsMatchingDrugs()
    {
        // Seed with a specific drug
        var drug = new Drug { Id = Guid.NewGuid(), Name = "SearchableDrug", ChemicalName = "SearchableChemical" };
        await _context.Drugs.InsertOneAsync(drug);

        var results = await _repository.SearchDrugsAsync("Searchable");

        Assert.Contains(results, d => d.Id == drug.Id);

        // Cleanup
        await _context.Drugs.DeleteOneAsync(x => x.Id == drug.Id);
    }

    [Fact]
    public async Task ListDrugsAsync_ReturnsDrugsWithinLimits()
    {
        // Seed with multiple drugs
        var drugs = new List<Drug>
        {
            new Drug { Id = Guid.NewGuid(), Name = "Drug1", ChemicalName = "Chemical1" },
            new Drug { Id = Guid.NewGuid(), Name = "Drug2", ChemicalName = "Chemical2" }
        };
        await _context.Drugs.InsertManyAsync(drugs);

        var results = await _repository.ListDrugsAsync(2, 0);

        Assert.Equal(2, results.Count());

        // Cleanup
        foreach (var drug in drugs)
        {
            await _context.Drugs.DeleteOneAsync(x => x.Id == drug.Id);
        }
    }

    public void Dispose()
    {
        // Drop the test database after each test
        _context.Client.DropDatabase("MedPoint_TestDB");
    }
}
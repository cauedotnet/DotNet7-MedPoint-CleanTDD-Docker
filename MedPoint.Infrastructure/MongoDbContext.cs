using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MedPoint.Domain.Entities;

namespace MedPoint.Infrastructure;

public class MongoDbContext
{
    public IMongoCollection<User> Users { get; }
    public IMongoCollection<Drug> Drugs { get; }
    public IMongoCollection<Log> Logs { get; }

    public MongoClient Client;

    public MongoDbContext(IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("MongoDb");
        var dbName = conn.Split('/').Last();

        Client = new MongoClient(conn);
        var database = Client.GetDatabase(dbName);

        Users = database.GetCollection<User>("Users");
        Drugs = database.GetCollection<Drug>("Drugs");
        Logs = database.GetCollection<Log>("Logs");
    }

    public MongoDbContext(string conn, string db)
    {
        Client = new MongoClient(conn);
        var database = Client.GetDatabase(db);

        Users = database.GetCollection<User>("Users");
        Drugs = database.GetCollection<Drug>("Drugs");
        Logs = database.GetCollection<Log>("Logs");
    }
}

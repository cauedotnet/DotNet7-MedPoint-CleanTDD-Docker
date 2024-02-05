using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using MedPoint.Application.Interfaces;
using MedPoint.Application.Services;
using MedPoint.Domain.Entities;
using MedPoint.Domain.Interfaces;
using MedPoint.Infrastructure;
using MedPoint.Infrastructure.Repositories;
using MedPoint.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();

// Infra -> MongoDb Context
builder.Services.AddScoped<MongoDbContext>();

// Application Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDrugService, DrugService>();
builder.Services.AddScoped<ILogService, LogService>();

// Domain Interfaces -> Infrastructure Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDrugRepository, DrugRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();

// External Services
builder.Services.AddScoped<IFDA_ApiService, FDA_ApiService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

AddSwaggerGenWithJWTToken(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseDefaultFiles();
app.UseStaticFiles();

// MongoDB Initialization and Seeding
InitializeMongoDB(app.Services);

app.Run();


void InitializeMongoDB(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var services = scope.ServiceProvider;

    var mongoDbContext = services.GetRequiredService<MongoDbContext>();
    // Ensure indexes
    CreateIndexes(mongoDbContext);
    // Seed data
    SeedData(mongoDbContext).Wait();
}

// Seeds Data for demo purpose
async Task SeedData(MongoDbContext context)
{
    if (!await context.Drugs.Find(_ => true).AnyAsync())
    {
        var drugs = new List<Drug>
        {
            new Drug
            {
                Name = "Nexium",
                ChemicalName = "esomeprazole",
                Manufacturer = "Accord Healthcare Inc",
                Description = "esomeprazole 40 MG Injection [Nexium]",
                DosageAndAdministration = "GERD with Erosive Esophagitis (2.1): ...",
            },
            new Drug
            {
                Name = "Dexilant",
                ChemicalName = "dexlansoprazole",
                Manufacturer = "Takeda Pharma",
                Description = "dexlansoprazole 30 MG Delayed Release Oral Capsule [Dexilant]",
                DosageAndAdministration = "Recommended dosage in patients 12 years of age and older: ...",
            }
        };

        await context.Drugs.InsertManyAsync(drugs);
    }

    if (!await context.Users.Find(_ => true).AnyAsync())
    {
        var users = new List<User>
        {
            new User
            {
                Username="admin",
                Name="Admin",
                Email="admin@admin.com",
                PasswordHash=BCrypt.Net.BCrypt.HashPassword("admin"),
                Role ="Admin"
            },
        };

        await context.Users.InsertManyAsync(users);
    }
}

void CreateIndexes(MongoDbContext context)
{
    context.Drugs.Indexes.CreateOne(new CreateIndexModel<Drug>(
        Builders<Drug>.IndexKeys.Ascending(d => d.Name)));

    context.Logs.Indexes.CreateOne(new CreateIndexModel<Log>(
        Builders<Log>.IndexKeys.Descending(d => d.Timestamp)));
}

static void AddSwaggerGenWithJWTToken(WebApplicationBuilder builder)
{
    //builder.Services.AddSwaggerGen();

    // Add Swagger generation with security definitions
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

        // Define the OAuth2.0 scheme that's being used (JWT Bearer)
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
    });
}
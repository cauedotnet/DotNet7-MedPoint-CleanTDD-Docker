using MedPoint.Application.Interfaces;
using MedPoint.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Infrastructure.Services;

public class FDA_ApiService : IFDA_ApiService
{
    private readonly HttpClient _httpClient;

    public FDA_ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ValidateDrugAsync(Drug drug)
    {
        // Simulate network latency
        await Task.Delay(1000);

        // Example of how an actual call might look (commented out)
        /*
        var response = await _httpClient.GetAsync($"https://api.fda.gov/drug/{drug.Id}");
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to validate drug with FDA.");
        }
        */
    }
}

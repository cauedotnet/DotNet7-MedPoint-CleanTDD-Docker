using Amazon.Runtime;
using MedPoint.Domain.Entities;
using MedPoint.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Tests.IntegrationTests
{
    public class FDA_ApiServiceTests
    {
        [Fact]
        public async Task ValidateDrugAsync_KnownDrug_ResponseSuccessful()
        {
            // Arrange
            var service = new FDA_ApiService(new HttpClient());
            var drug = new Drug { Id = Guid.NewGuid(), Name = "KnownDrug" };

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => service.ValidateDrugAsync(drug));

            // Assert no exception is thrown, indicating a successful validation
            Assert.Null(exception);
        }

        [Fact]
        public async Task ValidateDrugAsync_PerformanceTest_ResponseTimeUnder5Seconds()
        {
            // Arrange
            var service = new FDA_ApiService(new HttpClient()); // Use a real or mocked HttpClient as appropriate
            var drug = new Drug { Id = Guid.NewGuid(), Name = "AnyDrug" };
            var stopwatch = Stopwatch.StartNew();

            // Act
            await service.ValidateDrugAsync(drug);

            // Assert
            stopwatch.Stop();
            Assert.True(stopwatch.ElapsedMilliseconds < 5000, "Validation took longer than 5 seconds.");
        }
    }
}

using MedPoint.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Application.Interfaces
{
    public interface IFDA_ApiService
    {
        Task ValidateDrugAsync(Drug drug);
    }
}

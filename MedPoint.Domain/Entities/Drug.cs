using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoint.Domain.Entities;

public class Drug
{
    public Guid Id { get; set; } // Unique identifier for the drug
    public string Name { get; set; } // Commercial name of the drug
    public string ChemicalName { get; set; } // Scientific name of the compound
    public string Manufacturer { get; set; } // Manufacturer of the drug
    public string Description { get; set; } // Description of the drug
    public string DosageAndAdministration { get; set; } // Dosage and administration instructions
}

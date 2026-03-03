using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuskanMobile.Application.DTOs 
{
    public class TaxRateDropdownDto
    {
        public int TaxRateId { get; set; }
        public string TaxName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public string DisplayName => $"{TaxName} ({Rate}%)";
    }
}

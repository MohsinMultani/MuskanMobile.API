using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuskanMobile.Application.DTOs
{
    public class TaxRateDto
    {
        public int TaxRateId { get; set; }
        public string TaxName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Statistics
        public int ProductCount { get; set; }

        // For display
        public string DisplayName => $"{TaxName} ({Rate}%)";
    }
}

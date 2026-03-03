using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuskanMobile.Application.DTOs
{
    public class UpdateTaxRateDto
    {
        [Required]
        public int TaxRateId { get; set; }

        [Required(ErrorMessage = "Tax name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string TaxName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rate is required")]
        [Range(0, 100, ErrorMessage = "Rate must be between 0 and 100")]
        public decimal Rate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}

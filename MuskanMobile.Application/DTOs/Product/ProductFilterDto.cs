using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuskanMobile.Application.DTOs
{
    public class ProductFilterDto
    {
        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsActive { get; set; }
        public string? SearchTerm { get; set; }
        public bool? LowStock { get; set; }
        public int? TaxRateId { get; set; }
    }
}

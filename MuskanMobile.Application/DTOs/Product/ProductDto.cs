using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuskanMobile.Application.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? SKU { get; set; }
        public string? Barcode { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public int? TaxRateId { get; set; }
        public string? TaxRateName { get; set; }
        public decimal? TaxPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Computed properties
        public decimal PriceWithTax => TaxPercentage.HasValue
            ? Price + (Price * TaxPercentage.Value / 100)
            : Price;

        public string StockStatus => StockQuantity > 10 ? "In Stock"
            : StockQuantity > 0 ? "Low Stock"
            : "Out of Stock";
    }


}

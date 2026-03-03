using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuskanMobile.Application.DTOs
{
    public class ProductStockDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string StockStatus { get; set; } = string.Empty;
        public int? ReorderLevel { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuskanMobile.Application.DTOs
{
    public class UpdateSalesOrderStatusDto
    {
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateSalesPaymentStatusDto  //UpdatePaymentStatusDto
    {
        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class SalesOrderSummaryDto
    {
        public int SalesOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal GrandTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public int TotalQuantity { get; set; }
    }
}

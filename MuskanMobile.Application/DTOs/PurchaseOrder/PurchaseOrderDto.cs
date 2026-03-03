using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuskanMobile.Application.DTOs
{
    public class PurchaseOrderDto
    {
        public int PurchaseOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public List<PurchaseItemDto> PurchaseItems { get; set; } = new List<PurchaseItemDto>();

        // Computed properties
        public int TotalItems => PurchaseItems.Count;
        public int TotalQuantity => PurchaseItems.Sum(i => i.Quantity);
    }


    public class UpdatePurchaseOrderStatusDto
    {
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }

    public class UpdatePurchasePaymentStatusDto
    {
        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class ReceivePurchaseOrderDto
    {
        [Required]
        public int PurchaseOrderId { get; set; }

        public string? ReceivedNotes { get; set; }
    }

    public class PurchaseOrderSummaryDto
    {
        public int PurchaseOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public decimal GrandTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public int TotalQuantity { get; set; }
    }
}

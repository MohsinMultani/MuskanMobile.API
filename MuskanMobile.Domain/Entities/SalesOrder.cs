//using MuskanMobile.Domain.Common;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace MuskanMobile.Domain.Entities;

//[Table("SalesOrders", Schema = "Muskan")]
//public partial class SalesOrder : BaseEntity
//{
//    [Key]
//    public int SalesOrderId { get; set; }

//    public int CustomerId { get; set; }

//    [Column(TypeName = "datetime")]
//    public DateTime? OrderDate { get; set; }

//    [Column(TypeName = "decimal(18, 2)")]
//    public decimal? TotalAmount { get; set; }

//    [StringLength(50)]
//    public string Status { get; set; }

//    [ForeignKey("CustomerId")]
//    [InverseProperty("SalesOrders")]
//    public virtual Customer Customer { get; set; }

//    [InverseProperty("SalesOrder")]
//    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

//    [InverseProperty("SalesOrder")]
//    public virtual ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();
//}


using MuskanMobile.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuskanMobile.Domain.Entities
{
    [Table("SalesOrders", Schema = "Muskan")]
    public class SalesOrder : BaseEntity
    {
        [Key]
        public int SalesOrderId { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotal { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Processing, Shipped, Delivered, Cancelled

        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Partial, Refunded

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();
    }
}
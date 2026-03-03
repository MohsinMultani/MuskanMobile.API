//using MuskanMobile.Domain.Common;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;


//namespace MuskanMobile.Domain.Entities;

//[Table("SalesItems", Schema = "Muskan")]
//public partial class SalesItem : BaseEntity
//{
//    [Key]
//    public int SalesItemId { get; set; }

//    public int SalesOrderId { get; set; }

//    public int ProductId { get; set; }

//    public int Quantity { get; set; }

//    [Column(TypeName = "decimal(18, 2)")]
//    public decimal UnitPrice { get; set; }

//    [Column(TypeName = "decimal(18, 2)")]
//    public decimal? TaxAmount { get; set; }

//    [ForeignKey("ProductId")]
//    [InverseProperty("SalesItems")]
//    public virtual Product Product { get; set; }

//    [ForeignKey("SalesOrderId")]
//    [InverseProperty("SalesItems")]
//    public virtual SalesOrder SalesOrder { get; set; }
//}


using MuskanMobile.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuskanMobile.Domain.Entities
{
    [Table("SalesItems", Schema = "Muskan")]
    public class SalesItem : BaseEntity
    {
        [Key]
        public int SalesItemId { get; set; }

        [Required]
        public int SalesOrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("SalesOrderId")]
        public virtual SalesOrder? SalesOrder { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
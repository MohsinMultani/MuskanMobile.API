//using MuskanMobile.Domain.Common;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace MuskanMobile.Domain.Entities;

//[Table("Products", Schema = "Muskan")]
//public partial class Product : BaseEntity
//{
//    [Key]
//    public int ProductId { get; set; }

//    [Required]
//    [StringLength(150)]
//    public string ProductName { get; set; }

//    [Column(TypeName = "decimal(18, 2)")]
//    public decimal Price { get; set; }

//    public int StockQuantity { get; set; }

//    public int CategoryId { get; set; }

//    public int? SupplierId { get; set; }

//    public int TaxRateId { get; set; }

//    //[Column(TypeName = "datetime")]
//    //public DateTime? CreatedDate { get; set; }

//    [ForeignKey("CategoryId")]
//    [InverseProperty("Products")]
//    public virtual Category Category { get; set; }

//    [InverseProperty("Product")]
//    public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

//    [InverseProperty("Product")]
//    public virtual ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();

//    [ForeignKey("SupplierId")]
//    [InverseProperty("Products")]
//    public virtual Supplier Supplier { get; set; }

//    [ForeignKey("TaxRateId")]
//    [InverseProperty("Products")]
//    public virtual TaxRate TaxRate { get; set; }
//}

using MuskanMobile.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuskanMobile.Domain.Entities
{
    [Table("Products", Schema = "Muskan")]
    public class Product : BaseEntity
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public string? SKU { get; set; }  // Stock Keeping Unit

        public string? Barcode { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int? SupplierId { get; set; }

        public int? TaxRateId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }

        [ForeignKey("TaxRateId")]
        public virtual TaxRate? TaxRate { get; set; }

        // Inverse navigation
        [InverseProperty("Product")]
        public virtual ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();

        [InverseProperty("Product")]
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}

//using MuskanMobile.Domain.Common;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace MuskanMobile.Domain.Entities;

//[Table("Suppliers", Schema = "Muskan")]
//public partial class Supplier : BaseEntity
//{
//    [Key]
//    public int SupplierId { get; set; }

//    [Required]
//    [StringLength(150)]
//    public string SupplierName { get; set; }

//    [StringLength(20)]
//    public string PhoneNumber { get; set; }

//    [StringLength(150)]
//    public string Email { get; set; }

//    [StringLength(300)]
//    public string Address { get; set; }

//    [Column("GSTNumber")]
//    [StringLength(50)]
//    public string Gstnumber { get; set; }

//    [Column(TypeName = "datetime")]
//    public DateTime? CreatedDate { get; set; }

//    public DateTime? ModifiedDate { get; set; }

//    [InverseProperty("Supplier")]
//    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

//    [InverseProperty("Supplier")]
//    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
//}



using MuskanMobile.Domain.Common;
using System.Collections.Generic;

namespace MuskanMobile.Domain.Entities
{
    public class Supplier : BaseEntity
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;

        // Only include fields that actually exist in your database
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        // ❌ REMOVE these if they don't exist in your DB
        // public string? City { get; set; }
        // public string? State { get; set; }
        // public string? ZipCode { get; set; }
        // public string? Country { get; set; }
        // public string? GSTNumber { get; set; }
        // public string? PANNumber { get; set; }

        public bool IsActive { get; set; }
        // CreatedDate and ModifiedDate from BaseEntity

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
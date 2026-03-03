//using MuskanMobile.Domain.Common;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace MuskanMobile.Domain.Entities;

//[Table("Customers", Schema = "Muskan")]
//public partial class Customer : BaseEntity
//{
//    [Key]
//    public int CustomerId { get; set; }

//    [Required]
//    [StringLength(150)]
//    public string FullName { get; set; }

//    [StringLength(20)]
//    public string PhoneNumber { get; set; }

//    [StringLength(150)]
//    public string Email { get; set; }

//    [StringLength(300)]
//    public string Address { get; set; }

//    //[Column(TypeName = "datetime")]
//    //public DateTime? CreatedDate { get; set; }

//    [InverseProperty("Customer")]
//    public virtual ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
//}


using MuskanMobile.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuskanMobile.Domain.Entities
{
    [Table("Customers", Schema = "Muskan")]
    public class Customer : BaseEntity
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? ZipCode { get; set; }

        [StringLength(50)]
        public string? Country { get; set; } = "India";

        public bool IsActive { get; set; } = true;

        // Navigation property
        [InverseProperty("Customer")]
        public virtual ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    }
}
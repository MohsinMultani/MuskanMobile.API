//using System;
//using System.ComponentModel.DataAnnotations;

//namespace MuskanMobile.Application.DTOs
//{
//    public class SupplierDto
//    {
//        public int SupplierId { get; set; }
//        public string SupplierName { get; set; } = string.Empty;
//        public string ContactPerson { get; set; } = string.Empty;
//        public string Phone { get; set; } = string.Empty;
//        public string Email { get; set; } = string.Empty;
//        public string Address { get; set; } = string.Empty;
//        public string City { get; set; } = string.Empty;
//        public string State { get; set; } = string.Empty;
//        public string ZipCode { get; set; } = string.Empty;
//        public string Country { get; set; } = string.Empty;
//        public string GSTNumber { get; set; } = string.Empty;
//        public string PANNumber { get; set; } = string.Empty;
//        public bool IsActive { get; set; }
//        public DateTime CreatedDate { get; set; }
//        public DateTime? ModifiedDate { get; set; }

//        // Statistics (optional, for UI)
//        public int ProductCount { get; set; }
//        public int PurchaseOrderCount { get; set; }
//    }

//    public class CreateSupplierDto
//    {
//        [Required(ErrorMessage = "Supplier name is required")]
//        [StringLength(100, MinimumLength = 2)]
//        public string SupplierName { get; set; } = string.Empty;

//        [StringLength(100)]
//        public string ContactPerson { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Phone number is required")]
//        [Phone]
//        [StringLength(20)]
//        public string Phone { get; set; } = string.Empty;

//        [EmailAddress]
//        [StringLength(100)]
//        public string Email { get; set; } = string.Empty;

//        [StringLength(200)]
//        public string Address { get; set; } = string.Empty;

//        [StringLength(50)]
//        public string City { get; set; } = string.Empty;

//        [StringLength(50)]
//        public string State { get; set; } = string.Empty;

//        [StringLength(20)]
//        public string ZipCode { get; set; } = string.Empty;

//        [StringLength(50)]
//        public string Country { get; set; } = "India";

//        [StringLength(50)]
//        public string GSTNumber { get; set; } = string.Empty;

//        [StringLength(50)]
//        public string PANNumber { get; set; } = string.Empty;

//        public bool IsActive { get; set; } = true;
//    }

//    public class UpdateSupplierDto
//    {
//        [Required]
//        public int SupplierId { get; set; }

//        [Required(ErrorMessage = "Supplier name is required")]
//        [StringLength(100, MinimumLength = 2)]
//        public string SupplierName { get; set; } = string.Empty;

//        [StringLength(100)]
//        public string ContactPerson { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Phone number is required")]
//        [Phone]
//        [StringLength(20)]
//        public string Phone { get; set; } = string.Empty;

//        [EmailAddress]
//        [StringLength(100)]
//        public string Email { get; set; } = string.Empty;

//        [StringLength(200)]
//        public string Address { get; set; } = string.Empty;

//        [StringLength(50)]
//        public string City { get; set; } = string.Empty;

//        [StringLength(50)]
//        public string State { get; set; } = string.Empty;

//        [StringLength(20)]
//        public string ZipCode { get; set; } = string.Empty;

//        [StringLength(50)]
//        public string Country { get; set; } = string.Empty;

//        [StringLength(50)]
//        public string GSTNumber { get; set; } = string.Empty;

//        [StringLength(50)]
//        public string PANNumber { get; set; } = string.Empty;

//        public bool IsActive { get; set; }
//    }

//    public class SupplierDropdownDto
//    {
//        public int SupplierId { get; set; }
//        public string SupplierName { get; set; } = string.Empty;
//        public string Phone { get; set; } = string.Empty;
//    }
//}

using System;
using System.ComponentModel.DataAnnotations;

namespace MuskanMobile.Application.DTOs
{
    public class SupplierDto
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Statistics
        public int ProductCount { get; set; }
        public int PurchaseOrderCount { get; set; }
    }

    public class CreateSupplierDto
    {
        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string SupplierName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        // ❌ Removed all non-existent fields
    }

    public class UpdateSupplierDto
    {
        [Required]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string SupplierName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public bool IsActive { get; set; }

        // ❌ Removed all non-existent fields
    }

    public class SupplierDropdownDto
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}
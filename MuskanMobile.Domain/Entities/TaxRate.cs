//using MuskanMobile.Domain.Common;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;


//namespace MuskanMobile.Domain.Entities;

//[Table("TaxRates", Schema = "Muskan")]
//public partial class TaxRate : BaseEntity
//{
//    [Key]
//    public int TaxRateId { get; set; }

//    [Required]
//    [StringLength(50)]
//    public string TaxName { get; set; }

//    [Column(TypeName = "decimal(5, 2)")]
//    public decimal Percentage { get; set; }

//    [InverseProperty("TaxRate")]
//    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
//}


using MuskanMobile.Domain.Common;
using System.Collections.Generic;

namespace MuskanMobile.Domain.Entities
{
    public class TaxRate : BaseEntity
    {
        public int TaxRateId { get; set; }
        public string TaxName { get; set; } = string.Empty;
        public decimal Rate { get; set; }  // Clean name (after rename)
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
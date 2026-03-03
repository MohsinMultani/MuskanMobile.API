using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuskanMobile.Application.DTOs
{
    public class UpdateSalesOrderDto
    {
        [Required]
        public int SalesOrderId { get; set; }

        public string? Notes { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}

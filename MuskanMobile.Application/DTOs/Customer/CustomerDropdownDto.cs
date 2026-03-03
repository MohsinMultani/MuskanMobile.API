using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuskanMobile.Application.DTOs.Customer
{
    public class CustomerDropdownDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string DisplayName => $"{CustomerName} ({Phone})".Replace(" ()", "");
    }
}

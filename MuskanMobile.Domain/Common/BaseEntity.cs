using System;

namespace MuskanMobile.Domain.Common
{
    public abstract class BaseEntity
    {
        // Common ID property if all entities use int IDs
        // (Optional - only if all your entities have int Id)
        // public int Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // You can also add:
        // public string? CreatedBy { get; set; }
        // public string? ModifiedBy { get; set; }
        // public bool IsDeleted { get; set; }
    }
}
using MuskanMobile.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuskanMobile.Application.Interfaces
{
    public interface ISupplierService
    {
        // Basic CRUD
        Task<IEnumerable<SupplierDto>> GetAllAsync();
        Task<SupplierDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateSupplierDto dto);
        Task UpdateAsync(int id, UpdateSupplierDto dto);
        Task DeleteAsync(int id);

        // Additional useful methods
        Task<IEnumerable<SupplierDropdownDto>> GetSupplierDropdownAsync();
        Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync();
        Task<bool> ExistsAsync(int id);
        Task<bool> HasProductsAsync(int supplierId);
        Task<IEnumerable<SupplierDto>> SearchSuppliersAsync(string searchTerm);
    }
}
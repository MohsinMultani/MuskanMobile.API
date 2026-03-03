using MuskanMobile.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuskanMobile.Application.Interfaces
{
    public interface ITaxRateService
    {
        // Basic CRUD
        Task<IEnumerable<TaxRateDto>> GetAllAsync();
        Task<TaxRateDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateTaxRateDto dto);
        Task UpdateAsync(int id, UpdateTaxRateDto dto);
        Task DeleteAsync(int id);

        // Additional useful methods
        Task<IEnumerable<TaxRateDropdownDto>> GetTaxRateDropdownAsync();
        Task<IEnumerable<TaxRateDto>> GetActiveTaxRatesAsync();
        Task<bool> ExistsAsync(int id);
        Task<bool> HasProductsAsync(int taxRateId);
        Task<bool> IsTaxNameUniqueAsync(string taxName, int? excludeId = null);
        Task<IEnumerable<TaxRateDto>> SearchTaxRatesAsync(string searchTerm);
    }
}
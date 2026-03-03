using MuskanMobile.Application.DTOs;
using MuskanMobile.Application.DTOs.Customer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuskanMobile.Application.Interfaces
{
    public interface ICustomerService
    {
        // Basic CRUD
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<CustomerDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateCustomerDto dto);
        Task UpdateAsync(int id, UpdateCustomerDto dto);
        Task DeleteAsync(int id);

        // Additional useful methods
        Task<IEnumerable<CustomerDropdownDto>> GetCustomerDropdownAsync();
        Task<IEnumerable<CustomerDto>> GetActiveCustomersAsync();
        Task<bool> ExistsAsync(int id);
        Task<bool> HasSalesOrdersAsync(int customerId);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        Task<bool> IsPhoneUniqueAsync(string phone, int? excludeId = null);
        Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm);
        Task<IEnumerable<CustomerDto>> GetCustomersByCityAsync(string city);
    }
}
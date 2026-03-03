//using MuskanMobile.Application.DTOs;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MuskanMobile.Application.Interfaces
//{
//    public interface IProductService
//    {
//        Task<IEnumerable<ProductDto>> GetAllAsync();
//        Task<ProductDto> GetByIdAsync(int id);
//        Task<int> CreateAsync(CreateProductDto dto);
//        Task UpdateAsync(int id, UpdateProductDto dto);
//        Task DeleteAsync(int id);
//    }
//}


using MuskanMobile.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuskanMobile.Application.Interfaces
{
    public interface IProductService
    {
        // Basic CRUD
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateProductDto dto);
        Task UpdateAsync(int id, UpdateProductDto dto);
        Task DeleteAsync(int id);

        // Filtering and search
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductDto>> GetProductsBySupplierAsync(int supplierId);
        Task<IEnumerable<ProductDto>> GetProductsByTaxRateAsync(int taxRateId);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<ProductDto>> FilterProductsAsync(ProductFilterDto filter);

        // Stock management
        Task<ProductStockDto?> GetProductStockAsync(int id);
        Task<IEnumerable<ProductStockDto>> GetLowStockProductsAsync(int threshold = 10);
        Task<bool> UpdateStockAsync(int id, int quantity, bool isAddition);

        // Utility methods
        Task<IEnumerable<ProductDropdownDto>> GetProductDropdownAsync();
        Task<bool> ExistsAsync(int id);
        Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null);
        Task<IEnumerable<ProductDto>> GetActiveProductsAsync();
        Task<decimal> GetProductPriceWithTaxAsync(int id);
    }
}
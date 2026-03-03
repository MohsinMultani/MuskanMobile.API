//using AutoMapper;
//using MuskanMobile.Application.DTOs;
//using MuskanMobile.Application.Interfaces;
//using MuskanMobile.Domain.Entities;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System;

//namespace MuskanMobile.Application.Services;

//public class ProductService : IProductService
//{
//    private readonly IRepository<Product> _repository;
//    private readonly IMapper _mapper;

//    public ProductService(
//        IRepository<Product> repository,
//        IMapper mapper)
//    {
//        _repository = repository;
//        _mapper = mapper;
//    }

//    public async Task<IEnumerable<ProductDto>> GetAllAsync()
//    {
//        var products = await _repository.GetAllAsync();
//        return _mapper.Map<IEnumerable<ProductDto>>(products);
//    }

//    public async Task<ProductDto?> GetByIdAsync(int id)
//    {
//        var product = await _repository.GetByIdAsync(id);
//        if (product == null) return null;

//        return _mapper.Map<ProductDto>(product);
//    }

//    public async Task<int> CreateAsync(CreateProductDto dto)
//    {
//        var product = _mapper.Map<Product>(dto);

//        product.CreatedDate = DateTime.UtcNow; // ensure set

//        await _repository.AddAsync(product);

//        return product.ProductId;
//    }

//    public async Task UpdateAsync(int id, UpdateProductDto dto)
//    {
//        var product = await _repository.GetByIdAsync(id);
//        if (product == null)
//            throw new Exception("Product not found");

//        _mapper.Map(dto, product);
//        _repository.Update(product);
//    }

//    public async Task DeleteAsync(int id)
//    {
//        var product = await _repository.GetByIdAsync(id);
//        if (product == null)
//            throw new Exception("Product not found");

//        _repository.Delete(product);
//    }
//}


using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MuskanMobile.Application.DTOs;
using MuskanMobile.Application.Interfaces;
using MuskanMobile.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuskanMobile.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<TaxRate> _taxRateRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IRepository<Product> repository,
            IRepository<Category> categoryRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<TaxRate> taxRateRepository,
            IMapper mapper)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
            _taxRateRepository = taxRateRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repository.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.TaxRate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.TaxRate)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return null;

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<int> CreateAsync(CreateProductDto dto)
        {
            // Validate Category exists
            var categoryExists = await _categoryRepository.GetQueryable()
                .AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
                throw new Exception($"Category with ID {dto.CategoryId} not found");

            // Validate Supplier if provided
            if (dto.SupplierId.HasValue)
            {
                var supplierExists = await _supplierRepository.GetQueryable()
                    .AnyAsync(s => s.SupplierId == dto.SupplierId);
                if (!supplierExists)
                    throw new Exception($"Supplier with ID {dto.SupplierId} not found");
            }

            // Validate TaxRate if provided
            if (dto.TaxRateId.HasValue)
            {
                var taxRateExists = await _taxRateRepository.GetQueryable()
                    .AnyAsync(t => t.TaxRateId == dto.TaxRateId);
                if (!taxRateExists)
                    throw new Exception($"Tax rate with ID {dto.TaxRateId} not found");
            }

            // Check SKU uniqueness if provided
            if (!string.IsNullOrWhiteSpace(dto.SKU))
            {
                var isSkuUnique = await IsSkuUniqueAsync(dto.SKU);
                if (!isSkuUnique)
                    throw new Exception($"SKU '{dto.SKU}' already exists");
            }

            var product = _mapper.Map<Product>(dto);
            await _repository.AddAsync(product);
            return product.ProductId;
        }

        public async Task UpdateAsync(int id, UpdateProductDto dto)
        {
            if (id != dto.ProductId)
                throw new Exception("ID mismatch");

            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                throw new Exception("Product not found");

            // Validate Category exists
            var categoryExists = await _categoryRepository.GetQueryable()
                .AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
                throw new Exception($"Category with ID {dto.CategoryId} not found");

            // Validate Supplier if provided
            if (dto.SupplierId.HasValue)
            {
                var supplierExists = await _supplierRepository.GetQueryable()
                    .AnyAsync(s => s.SupplierId == dto.SupplierId);
                if (!supplierExists)
                    throw new Exception($"Supplier with ID {dto.SupplierId} not found");
            }

            // Validate TaxRate if provided
            if (dto.TaxRateId.HasValue)
            {
                var taxRateExists = await _taxRateRepository.GetQueryable()
                    .AnyAsync(t => t.TaxRateId == dto.TaxRateId);
                if (!taxRateExists)
                    throw new Exception($"Tax rate with ID {dto.TaxRateId} not found");
            }

            // Check SKU uniqueness if changed
            if (!string.IsNullOrWhiteSpace(dto.SKU) && dto.SKU != product.SKU)
            {
                var isSkuUnique = await IsSkuUniqueAsync(dto.SKU, id);
                if (!isSkuUnique)
                    throw new Exception($"SKU '{dto.SKU}' already exists");
            }

            _mapper.Map(dto, product);
            _repository.Update(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                throw new Exception("Product not found");

            // Check if product has sales or purchase items
            var hasSalesItems = await _repository.GetQueryable()
                .AnyAsync(p => p.ProductId == id && p.SalesItems.Any());

            var hasPurchaseItems = await _repository.GetQueryable()
                .AnyAsync(p => p.ProductId == id && p.PurchaseItems.Any());

            if (hasSalesItems || hasPurchaseItems)
                throw new Exception("Cannot delete product with existing sales or purchase history");

            _repository.Delete(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _repository.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.TaxRate)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsBySupplierAsync(int supplierId)
        {
            var products = await _repository.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.TaxRate)
                .Where(p => p.SupplierId == supplierId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByTaxRateAsync(int taxRateId)
        {
            var products = await _repository.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.TaxRate)
                .Where(p => p.TaxRateId == taxRateId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower().Trim();

            var products = await _repository.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.TaxRate)
                .Where(p =>
                    p.ProductName.ToLower().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                    (p.SKU != null && p.SKU.ToLower().Contains(searchTerm)) ||
                    (p.Barcode != null && p.Barcode.Contains(searchTerm)) ||
                    (p.Category != null && p.Category.CategoryName.ToLower().Contains(searchTerm)) ||
                    (p.Supplier != null && p.Supplier.SupplierName.ToLower().Contains(searchTerm)))
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> FilterProductsAsync(ProductFilterDto filter)
        {
            var query = _repository.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.TaxRate)
                .AsQueryable();

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId);

            if (filter.SupplierId.HasValue)
                query = query.Where(p => p.SupplierId == filter.SupplierId);

            if (filter.TaxRateId.HasValue)
                query = query.Where(p => p.TaxRateId == filter.TaxRateId);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice);

            if (filter.IsActive.HasValue)
                query = query.Where(p => p.IsActive == filter.IsActive);

            if (filter.LowStock.HasValue && filter.LowStock.Value)
                query = query.Where(p => p.StockQuantity <= 10);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(p =>
                    p.ProductName.ToLower().Contains(term) ||
                    (p.SKU != null && p.SKU.ToLower().Contains(term)));
            }

            var products = await query.ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductStockDto?> GetProductStockAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductStockDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                StockQuantity = product.StockQuantity,
                StockStatus = product.StockQuantity > 10 ? "In Stock"
                    : product.StockQuantity > 0 ? "Low Stock"
                    : "Out of Stock",
                ReorderLevel = 10 // You can make this configurable
            };
        }

        public async Task<IEnumerable<ProductStockDto>> GetLowStockProductsAsync(int threshold = 10)
        {
            var products = await _repository.GetQueryable()
                .Where(p => p.StockQuantity <= threshold && p.StockQuantity > 0)
                .ToListAsync();

            return products.Select(p => new ProductStockDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                StockQuantity = p.StockQuantity,
                StockStatus = "Low Stock",
                ReorderLevel = threshold
            });
        }

        public async Task<bool> UpdateStockAsync(int id, int quantity, bool isAddition)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                throw new Exception("Product not found");

            if (isAddition)
                product.StockQuantity += quantity;
            else
            {
                if (product.StockQuantity < quantity)
                    throw new Exception("Insufficient stock");
                product.StockQuantity -= quantity;
            }

            _repository.Update(product);
            return true;
        }

        public async Task<IEnumerable<ProductDropdownDto>> GetProductDropdownAsync()
        {
            return await _repository.GetQueryable()
                .Where(p => p.IsActive && p.StockQuantity > 0)
                .OrderBy(p => p.ProductName)
                .Select(p => new ProductDropdownDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price
                })
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.GetQueryable().AnyAsync(p => p.ProductId == id);
        }

        public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(sku)) return true;

            var query = _repository.GetQueryable()
                .Where(p => p.SKU != null && p.SKU.ToLower() == sku.ToLower());

            if (excludeId.HasValue)
                query = query.Where(p => p.ProductId != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
        {
            var products = await _repository.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.TaxRate)
                .Where(p => p.IsActive)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<decimal> GetProductPriceWithTaxAsync(int id)
        {
            var product = await _repository.GetQueryable()
                .Include(p => p.TaxRate)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                throw new Exception("Product not found");

            if (product.TaxRate == null)
                return product.Price;

            return product.Price + (product.Price * product.TaxRate.Rate / 100);
        }
    }
}
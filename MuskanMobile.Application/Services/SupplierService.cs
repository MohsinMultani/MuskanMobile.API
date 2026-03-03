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
    public class SupplierService : ISupplierService
    {
        private readonly IRepository<Supplier> _repository;
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public SupplierService(
            IRepository<Supplier> repository,
            IRepository<Product> productRepository,
            IMapper mapper)
        {
            _repository = repository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllAsync()
        {
            var suppliers = await _repository.GetAllAsync();
            var supplierDtos = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);

            // Enrich with product counts
            foreach (var dto in supplierDtos)
            {
                dto.ProductCount = await _productRepository.GetQueryable()
                    .CountAsync(p => p.SupplierId == dto.SupplierId);
            }

            return supplierDtos;
        }

        public async Task<SupplierDto?> GetByIdAsync(int id)
        {
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier == null) return null;

            var dto = _mapper.Map<SupplierDto>(supplier);

            // Get product count
            dto.ProductCount = await _productRepository.GetQueryable()
                .CountAsync(p => p.SupplierId == id);

            return dto;
        }

        public async Task<int> CreateAsync(CreateSupplierDto dto)
        {
            // Check if supplier with same name exists
            var existing = await _repository.GetQueryable()
                .FirstOrDefaultAsync(s => s.SupplierName.ToLower() == dto.SupplierName.ToLower());

            if (existing != null)
                throw new Exception("Supplier with this name already exists");

            var supplier = _mapper.Map<Supplier>(dto);
            //supplier.CreatedDate = DateTime.UtcNow;

            await _repository.AddAsync(supplier);
            return supplier.SupplierId;
        }

        public async Task UpdateAsync(int id, UpdateSupplierDto dto)
        {
            if (id != dto.SupplierId)
                throw new Exception("ID mismatch");

            var supplier = await _repository.GetByIdAsync(id);
            if (supplier == null)
                throw new Exception("Supplier not found");

            // Check name uniqueness (excluding current supplier)
            var existing = await _repository.GetQueryable()
                .FirstOrDefaultAsync(s => s.SupplierName.ToLower() == dto.SupplierName.ToLower()
                                        && s.SupplierId != id);

            if (existing != null)
                throw new Exception("Supplier with this name already exists");

            _mapper.Map(dto, supplier);
            //supplier.ModifiedDate = DateTime.UtcNow;

            _repository.Update(supplier);
        }

        public async Task DeleteAsync(int id)
        {
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier == null)
                throw new Exception("Supplier not found");

            // Check if supplier has products
            var hasProducts = await _productRepository.GetQueryable()
                .AnyAsync(p => p.SupplierId == id);

            if (hasProducts)
                throw new Exception("Cannot delete supplier with associated products");

            _repository.Delete(supplier);
        }

        public async Task<IEnumerable<SupplierDropdownDto>> GetSupplierDropdownAsync()
        {
            return await _repository.GetQueryable()
                .Where(s => s.IsActive)
                .OrderBy(s => s.SupplierName)
                .Select(s => new SupplierDropdownDto
                {
                    SupplierId = s.SupplierId,
                    SupplierName = s.SupplierName,
                    Phone = s.Phone
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync()
        {
            var suppliers = await _repository.GetQueryable()
                .Where(s => s.IsActive)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.GetQueryable().AnyAsync(s => s.SupplierId == id);
        }

        public async Task<bool> HasProductsAsync(int supplierId)
        {
            return await _productRepository.GetQueryable()
                .AnyAsync(p => p.SupplierId == supplierId);
        }

        public async Task<IEnumerable<SupplierDto>> SearchSuppliersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower().Trim();

            var suppliers = await _repository.GetQueryable()
                .Where(s =>
                    s.SupplierName.ToLower().Contains(searchTerm) ||
                    (s.ContactPerson != null && s.ContactPerson.ToLower().Contains(searchTerm)) ||
                    (s.Phone != null && s.Phone.Contains(searchTerm)) ||
                    (s.Email != null && s.Email.ToLower().Contains(searchTerm)) ||
                    (s.Address != null && s.Address.ToLower().Contains(searchTerm))
                // ❌ Removed City, State, GSTNumber, PANNumber searches
                )
                .OrderBy(s => s.SupplierName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }
    }
}
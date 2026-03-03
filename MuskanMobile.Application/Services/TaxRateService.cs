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
    public class TaxRateService : ITaxRateService
    {
        private readonly IRepository<TaxRate> _repository;
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public TaxRateService(
            IRepository<TaxRate> repository,
            IRepository<Product> productRepository,
            IMapper mapper)
        {
            _repository = repository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaxRateDto>> GetAllAsync()
        {
            var taxRates = await _repository.GetAllAsync();
            var taxRateDtos = _mapper.Map<IEnumerable<TaxRateDto>>(taxRates);

            // Enrich with product counts
            foreach (var dto in taxRateDtos)
            {
                dto.ProductCount = await _productRepository.GetQueryable()
                    .CountAsync(p => p.TaxRateId == dto.TaxRateId);
            }

            return taxRateDtos.OrderBy(t => t.TaxName);
        }

        public async Task<TaxRateDto?> GetByIdAsync(int id)
        {
            var taxRate = await _repository.GetByIdAsync(id);
            if (taxRate == null) return null;

            var dto = _mapper.Map<TaxRateDto>(taxRate);

            dto.ProductCount = await _productRepository.GetQueryable()
                .CountAsync(p => p.TaxRateId == id);

            return dto;
        }

        public async Task<int> CreateAsync(CreateTaxRateDto dto)
        {
            // Check if tax name is unique
            var isUnique = await IsTaxNameUniqueAsync(dto.TaxName);
            if (!isUnique)
                throw new Exception($"Tax rate '{dto.TaxName}' already exists");

            var taxRate = _mapper.Map<TaxRate>(dto);
            // CreatedDate auto-set by interceptor

            await _repository.AddAsync(taxRate);
            return taxRate.TaxRateId;
        }

        public async Task UpdateAsync(int id, UpdateTaxRateDto dto)
        {
            if (id != dto.TaxRateId)
                throw new Exception("ID mismatch");

            var taxRate = await _repository.GetByIdAsync(id);
            if (taxRate == null)
                throw new Exception("Tax rate not found");

            // Check name uniqueness (excluding current)
            var isUnique = await IsTaxNameUniqueAsync(dto.TaxName, id);
            if (!isUnique)
                throw new Exception($"Tax rate '{dto.TaxName}' already exists");

            _mapper.Map(dto, taxRate);
            // ModifiedDate auto-set by interceptor

            _repository.Update(taxRate);
        }

        public async Task DeleteAsync(int id)
        {
            var taxRate = await _repository.GetByIdAsync(id);
            if (taxRate == null)
                throw new Exception("Tax rate not found");

            // Check if tax rate is used by any products
            var hasProducts = await _productRepository.GetQueryable()
                .AnyAsync(p => p.TaxRateId == id);

            if (hasProducts)
                throw new Exception("Cannot delete tax rate that is assigned to products");

            _repository.Delete(taxRate);
        }

        public async Task<IEnumerable<TaxRateDropdownDto>> GetTaxRateDropdownAsync()
        {
            return await _repository.GetQueryable()
                .Where(t => t.IsActive)
                .OrderBy(t => t.TaxName)
                .Select(t => new TaxRateDropdownDto
                {
                    TaxRateId = t.TaxRateId,
                    TaxName = t.TaxName,
                    Rate = t.Rate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TaxRateDto>> GetActiveTaxRatesAsync()
        {
            var taxRates = await _repository.GetQueryable()
                .Where(t => t.IsActive)
                .OrderBy(t => t.TaxName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TaxRateDto>>(taxRates);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.GetQueryable().AnyAsync(t => t.TaxRateId == id);
        }

        public async Task<bool> HasProductsAsync(int taxRateId)
        {
            return await _productRepository.GetQueryable()
                .AnyAsync(p => p.TaxRateId == taxRateId);
        }

        public async Task<bool> IsTaxNameUniqueAsync(string taxName, int? excludeId = null)
        {
            var query = _repository.GetQueryable()
                .Where(t => t.TaxName.ToLower() == taxName.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.TaxRateId != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<TaxRateDto>> SearchTaxRatesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower().Trim();

            var taxRates = await _repository.GetQueryable()
                .Where(t =>
                    t.TaxName.ToLower().Contains(searchTerm) ||
                    (t.Description != null && t.Description.ToLower().Contains(searchTerm)) ||
                    t.Rate.ToString().Contains(searchTerm))
                .OrderBy(t => t.TaxName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TaxRateDto>>(taxRates);
        }
    }
}
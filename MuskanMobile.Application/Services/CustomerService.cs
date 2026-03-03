using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MuskanMobile.Application.DTOs;
using MuskanMobile.Application.DTOs.Customer;
using MuskanMobile.Application.Interfaces;
using MuskanMobile.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuskanMobile.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _repository;
        private readonly IRepository<SalesOrder> _salesOrderRepository;
        private readonly IMapper _mapper;

        public CustomerService(
            IRepository<Customer> repository,
            IRepository<SalesOrder> salesOrderRepository,
            IMapper mapper)
        {
            _repository = repository;
            _salesOrderRepository = salesOrderRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _repository.GetAllAsync();
            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);

            // Enrich with sales order counts
            foreach (var dto in customerDtos)
            {
                dto.SalesOrderCount = await _salesOrderRepository.GetQueryable()
                    .CountAsync(so => so.CustomerId == dto.CustomerId);
            }

            return customerDtos.OrderBy(c => c.CustomerName);
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null) return null;

            var dto = _mapper.Map<CustomerDto>(customer);

            dto.SalesOrderCount = await _salesOrderRepository.GetQueryable()
                .CountAsync(so => so.CustomerId == id);

            return dto;
        }

        public async Task<int> CreateAsync(CreateCustomerDto dto)
        {
            // Validate unique email if provided
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var isEmailUnique = await IsEmailUniqueAsync(dto.Email);
                if (!isEmailUnique)
                    throw new Exception($"Email '{dto.Email}' is already registered");
            }

            // Validate unique phone if provided
            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                var isPhoneUnique = await IsPhoneUniqueAsync(dto.Phone);
                if (!isPhoneUnique)
                    throw new Exception($"Phone '{dto.Phone}' is already registered");
            }

            var customer = _mapper.Map<Customer>(dto);
            // CreatedDate auto-set by interceptor

            await _repository.AddAsync(customer);
            return customer.CustomerId;
        }

        public async Task UpdateAsync(int id, UpdateCustomerDto dto)
        {
            if (id != dto.CustomerId)
                throw new Exception("ID mismatch");

            var customer = await _repository.GetByIdAsync(id);
            if (customer == null)
                throw new Exception("Customer not found");

            // Validate unique email if changed
            if (!string.IsNullOrWhiteSpace(dto.Email) &&
                dto.Email != customer.Email)
            {
                var isEmailUnique = await IsEmailUniqueAsync(dto.Email, id);
                if (!isEmailUnique)
                    throw new Exception($"Email '{dto.Email}' is already registered");
            }

            // Validate unique phone if changed
            if (!string.IsNullOrWhiteSpace(dto.Phone) &&
                dto.Phone != customer.Phone)
            {
                var isPhoneUnique = await IsPhoneUniqueAsync(dto.Phone, id);
                if (!isPhoneUnique)
                    throw new Exception($"Phone '{dto.Phone}' is already registered");
            }

            _mapper.Map(dto, customer);
            // ModifiedDate auto-set by interceptor

            _repository.Update(customer);
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null)
                throw new Exception("Customer not found");

            // Check if customer has sales orders
            var hasOrders = await _salesOrderRepository.GetQueryable()
                .AnyAsync(so => so.CustomerId == id);

            if (hasOrders)
                throw new Exception("Cannot delete customer with existing sales orders");

            _repository.Delete(customer);
        }

        public async Task<IEnumerable<CustomerDropdownDto>> GetCustomerDropdownAsync()
        {
            return await _repository.GetQueryable()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CustomerName)
                .Select(c => new CustomerDropdownDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    Phone = c.Phone
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerDto>> GetActiveCustomersAsync()
        {
            var customers = await _repository.GetQueryable()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.GetQueryable().AnyAsync(c => c.CustomerId == id);
        }

        public async Task<bool> HasSalesOrdersAsync(int customerId)
        {
            return await _salesOrderRepository.GetQueryable()
                .AnyAsync(so => so.CustomerId == customerId);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;

            var query = _repository.GetQueryable()
                .Where(c => c.Email != null && c.Email.ToLower() == email.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.CustomerId != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<bool> IsPhoneUniqueAsync(string phone, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(phone)) return true;

            var query = _repository.GetQueryable()
                .Where(c => c.Phone != null && c.Phone == phone);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.CustomerId != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower().Trim();

            var customers = await _repository.GetQueryable()
                .Where(c =>
                    c.CustomerName.ToLower().Contains(searchTerm) ||
                    (c.Phone != null && c.Phone.Contains(searchTerm)) ||
                    (c.Email != null && c.Email.ToLower().Contains(searchTerm)) ||
                    (c.City != null && c.City.ToLower().Contains(searchTerm)) ||
                    (c.State != null && c.State.ToLower().Contains(searchTerm)))
                .OrderBy(c => c.CustomerName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return await GetAllAsync();

            var customers = await _repository.GetQueryable()
                .Where(c => c.City != null && c.City.ToLower() == city.ToLower())
                .OrderBy(c => c.CustomerName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }
    }
}
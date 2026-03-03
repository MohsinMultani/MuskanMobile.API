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
    public class SalesOrderService : ISalesOrderService
    {
        private readonly IRepository<SalesOrder> _orderRepository;
        private readonly IRepository<SalesItem> _itemRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<TaxRate> _taxRateRepository;
        private readonly IMapper _mapper;
        private readonly ITelemetryService _telemetry;

        public SalesOrderService(
            IRepository<SalesOrder> orderRepository,
            IRepository<SalesItem> itemRepository,
            IRepository<Customer> customerRepository,
            IRepository<Product> productRepository,
            IRepository<TaxRate> taxRateRepository,
            IMapper mapper,
            ITelemetryService telemetry)
        {
            _orderRepository = orderRepository;
            _itemRepository = itemRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _taxRateRepository = taxRateRepository;
            _mapper = mapper;
            _telemetry = telemetry;
        }

        public async Task<IEnumerable<SalesOrderSummaryDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetQueryable()
                .Include(o => o.Customer)
                .Include(o => o.SalesItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new SalesOrderSummaryDto
            {
                SalesOrderId = o.SalesOrderId,
                OrderNumber = o.OrderNumber ?? "N/A",
                CustomerName = o.Customer?.CustomerName ?? "Unknown",
                OrderDate = o.OrderDate,
                GrandTotal = o.GrandTotal,
                Status = o.Status ?? "Pending",
                PaymentStatus = o.PaymentStatus ?? "Pending",
                ItemCount = o.SalesItems?.Count ?? 0,
                TotalQuantity = o.SalesItems?.Sum(i => i.Quantity) ?? 0
            });
        }

        public async Task<SalesOrderDto?> GetByIdAsync(int id)
        {
            var order = await _orderRepository.GetQueryable()
                .Include(o => o.Customer)
                .Include(o => o.SalesItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.SalesOrderId == id);

            if (order == null) return null;

            var orderDto = _mapper.Map<SalesOrderDto>(order);
            orderDto.CustomerName = order.Customer?.CustomerName;

            if (order.SalesItems != null)
            {
                foreach (var item in orderDto.SalesItems)
                {
                    var salesItem = order.SalesItems.FirstOrDefault(i => i.SalesItemId == item.SalesItemId);
                    item.ProductName = salesItem?.Product?.ProductName;
                }
            }

            return orderDto;
        }

        public async Task<int> CreateAsync(CreateSalesOrderDto dto)
        {
            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
            if (customer == null)
                throw new Exception($"Customer with ID {dto.CustomerId} not found");

            if (dto.SalesItems == null || !dto.SalesItems.Any())
                throw new Exception("At least one item is required");

            // Generate order number
            var orderNumber = await GenerateOrderNumberAsync();

            // Create order
            var order = new SalesOrder
            {
                OrderNumber = orderNumber,
                CustomerId = dto.CustomerId,
                OrderDate = dto.OrderDate,
                Notes = dto.Notes,
                Status = "Pending",
                PaymentStatus = "Pending",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            decimal totalAmount = 0;
            decimal totalDiscount = 0;
            decimal totalTax = 0;

            // Process each item
            foreach (var itemDto in dto.SalesItems)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {itemDto.ProductId} not found");

                // Check stock
                if (product.StockQuantity < itemDto.Quantity)
                    throw new Exception($"Insufficient stock for product {product.ProductName}. Available: {product.StockQuantity}");

                // Get tax rate - handle nullable TaxRateId
                decimal taxPercentage = 0;
                if (product.TaxRateId.HasValue)
                {
                    var taxRate = await _taxRateRepository.GetByIdAsync(product.TaxRateId.Value);
                    taxPercentage = taxRate?.Rate ?? 0;
                }

                // Calculate prices
                decimal unitPrice = itemDto.UnitPrice ?? product.Price;
                decimal itemDiscount = itemDto.DiscountAmount ?? 0;
                decimal itemSubtotal = unitPrice * itemDto.Quantity;
                decimal itemTax = (itemSubtotal - itemDiscount) * (taxPercentage / 100);
                decimal itemTotal = itemSubtotal - itemDiscount + itemTax;

                // Create sales item
                var item = new SalesItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = unitPrice,
                    DiscountAmount = itemDiscount,
                    TaxAmount = itemTax,
                    TotalAmount = itemTotal,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                order.SalesItems.Add(item);

                // Update product stock
                product.StockQuantity -= itemDto.Quantity;
                _productRepository.Update(product);

                // Accumulate totals
                totalAmount += itemSubtotal;
                totalDiscount += itemDiscount;
                totalTax += itemTax;
            }

            // Set order totals
            order.TotalAmount = totalAmount;
            order.DiscountAmount = totalDiscount;
            order.TaxAmount = totalTax;
            order.GrandTotal = totalAmount - totalDiscount + totalTax;

            // Save order
            await _orderRepository.AddAsync(order);

            _telemetry.TrackOrderCreated(order.SalesOrderId, order.GrandTotal, customer.CustomerName);

            return order.SalesOrderId;
        }

        public async Task UpdateAsync(int id, UpdateSalesOrderDto dto)
        {
            if (id != dto.SalesOrderId)
                throw new Exception("ID mismatch");

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new Exception("Sales order not found");

            // Only allow updates to certain fields
            order.Notes = dto.Notes ?? order.Notes;
            order.Status = dto.Status;
            order.PaymentStatus = dto.PaymentStatus;
            order.IsActive = dto.IsActive;
            order.ModifiedDate = DateTime.UtcNow;

            _orderRepository.Update(order);
        }

        public async Task UpdateStatusAsync(int id, UpdateSalesOrderStatusDto dto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new Exception("Sales order not found");

            // Validate status transition
            if (order.Status == "Cancelled" || order.Status == "Delivered")
                throw new Exception($"Cannot change status of {order.Status} orders");

            order.Status = dto.Status;
            order.ModifiedDate = DateTime.UtcNow;

            _orderRepository.Update(order);
        }

        public async Task UpdatePaymentStatusAsync(int id, UpdateSalesPaymentStatusDto dto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new Exception("Sales order not found");

            order.PaymentStatus = dto.PaymentStatus;
            order.ModifiedDate = DateTime.UtcNow;

            _orderRepository.Update(order);
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _orderRepository.GetQueryable()
                .Include(o => o.SalesItems)
                .FirstOrDefaultAsync(o => o.SalesOrderId == id);

            if (order == null)
                throw new Exception("Sales order not found");

            // Check if order can be deleted
            if (order.Status != "Pending" && order.Status != "Cancelled")
                throw new Exception("Only pending or cancelled orders can be deleted");

            // Restore product stock
            foreach (var item in order.SalesItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    _productRepository.Update(product);
                }
            }

            _orderRepository.Delete(order);
        }

        public async Task<IEnumerable<SalesOrderSummaryDto>> GetOrdersByCustomerAsync(int customerId)
        {
            var orders = await _orderRepository.GetQueryable()
                .Include(o => o.Customer)
                .Include(o => o.SalesItems)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new SalesOrderSummaryDto
            {
                SalesOrderId = o.SalesOrderId,
                OrderNumber = o.OrderNumber ?? "N/A",
                CustomerName = o.Customer?.CustomerName ?? "Unknown",
                OrderDate = o.OrderDate,
                GrandTotal = o.GrandTotal,
                Status = o.Status ?? "Pending",
                PaymentStatus = o.PaymentStatus ?? "Pending",
                ItemCount = o.SalesItems?.Count ?? 0,
                TotalQuantity = o.SalesItems?.Sum(i => i.Quantity) ?? 0
            });
        }

        public async Task<IEnumerable<SalesOrderSummaryDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetQueryable()
                .Include(o => o.Customer)
                .Include(o => o.SalesItems)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new SalesOrderSummaryDto
            {
                SalesOrderId = o.SalesOrderId,
                OrderNumber = o.OrderNumber ?? "N/A",
                CustomerName = o.Customer?.CustomerName ?? "Unknown",
                OrderDate = o.OrderDate,
                GrandTotal = o.GrandTotal,
                Status = o.Status ?? "Pending",
                PaymentStatus = o.PaymentStatus ?? "Pending",
                ItemCount = o.SalesItems?.Count ?? 0,
                TotalQuantity = o.SalesItems?.Sum(i => i.Quantity) ?? 0
            });
        }

        public async Task<IEnumerable<SalesOrderSummaryDto>> GetOrdersByStatusAsync(string status)
        {
            var orders = await _orderRepository.GetQueryable()
                .Include(o => o.Customer)
                .Include(o => o.SalesItems)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new SalesOrderSummaryDto
            {
                SalesOrderId = o.SalesOrderId,
                OrderNumber = o.OrderNumber ?? "N/A",
                CustomerName = o.Customer?.CustomerName ?? "Unknown",
                OrderDate = o.OrderDate,
                GrandTotal = o.GrandTotal,
                Status = o.Status ?? "Pending",
                PaymentStatus = o.PaymentStatus ?? "Pending",
                ItemCount = o.SalesItems?.Count ?? 0,
                TotalQuantity = o.SalesItems?.Sum(i => i.Quantity) ?? 0
            });
        }

        public async Task<IEnumerable<SalesOrderSummaryDto>> GetOrdersByPaymentStatusAsync(string paymentStatus)
        {
            var orders = await _orderRepository.GetQueryable()
                .Include(o => o.Customer)
                .Include(o => o.SalesItems)
                .Where(o => o.PaymentStatus == paymentStatus)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new SalesOrderSummaryDto
            {
                SalesOrderId = o.SalesOrderId,
                OrderNumber = o.OrderNumber ?? "N/A",
                CustomerName = o.Customer?.CustomerName ?? "Unknown",
                OrderDate = o.OrderDate,
                GrandTotal = o.GrandTotal,
                Status = o.Status ?? "Pending",
                PaymentStatus = o.PaymentStatus ?? "Pending",
                ItemCount = o.SalesItems?.Count ?? 0,
                TotalQuantity = o.SalesItems?.Sum(i => i.Quantity) ?? 0
            });
        }

        public async Task<IEnumerable<SalesOrderSummaryDto>> SearchOrdersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower().Trim();

            var orders = await _orderRepository.GetQueryable()
                .Include(o => o.Customer)
                .Include(o => o.SalesItems)
                .Where(o =>
                    o.OrderNumber.ToLower().Contains(searchTerm) ||
                    (o.Customer != null && o.Customer.CustomerName.ToLower().Contains(searchTerm)) ||
                    o.Status.ToLower().Contains(searchTerm) ||
                    o.PaymentStatus.ToLower().Contains(searchTerm))
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new SalesOrderSummaryDto
            {
                SalesOrderId = o.SalesOrderId,
                OrderNumber = o.OrderNumber ?? "N/A",
                CustomerName = o.Customer?.CustomerName ?? "Unknown",
                OrderDate = o.OrderDate,
                GrandTotal = o.GrandTotal,
                Status = o.Status ?? "Pending",
                PaymentStatus = o.PaymentStatus ?? "Pending",
                ItemCount = o.SalesItems?.Count ?? 0,
                TotalQuantity = o.SalesItems?.Sum(i => i.Quantity) ?? 0
            });
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _orderRepository.GetQueryable().AnyAsync(o => o.SalesOrderId == id);
        }

        public async Task<bool> CanCancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            // Can cancel if pending or confirmed
            return order.Status == "Pending" || order.Status == "Confirmed";
        }

        public async Task<decimal> CalculateOrderTotalAsync(int id)
        {
            var order = await _orderRepository.GetQueryable()
                .Include(o => o.SalesItems)
                .FirstOrDefaultAsync(o => o.SalesOrderId == id);

            if (order == null)
                throw new Exception("Sales order not found");

            return order.SalesItems?.Sum(i => i.TotalAmount) ?? 0;
        }

        public async Task<bool> ProcessPaymentAsync(int id, decimal amount)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new Exception("Sales order not found");

            if (amount >= order.GrandTotal)
            {
                order.PaymentStatus = "Paid";
            }
            else if (amount > 0)
            {
                order.PaymentStatus = "Partial";
            }

            order.ModifiedDate = DateTime.UtcNow;
            _orderRepository.Update(order);

            return true;
        }

        private async Task<string> GenerateOrderNumberAsync()
        {
            var today = DateTime.Today;
            var prefix = "SO-" + today.ToString("yyyyMMdd") + "-";

            var lastOrder = await _orderRepository.GetQueryable()
                .Where(o => o.OrderNumber.StartsWith(prefix))
                .OrderByDescending(o => o.OrderNumber)
                .FirstOrDefaultAsync();

            if (lastOrder == null)
            {
                return prefix + "0001";
            }

            var lastNumber = int.Parse(lastOrder.OrderNumber.Substring(prefix.Length));
            return prefix + (lastNumber + 1).ToString("D4");
        }
    }
}
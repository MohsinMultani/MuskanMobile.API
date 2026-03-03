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
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IRepository<PurchaseOrder> _orderRepository;
        private readonly IRepository<PurchaseItem> _itemRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<TaxRate> _taxRateRepository;
        private readonly IMapper _mapper;

        public PurchaseOrderService(
            IRepository<PurchaseOrder> orderRepository,
            IRepository<PurchaseItem> itemRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<Product> productRepository,
            IRepository<TaxRate> taxRateRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _itemRepository = itemRepository;
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
            _taxRateRepository = taxRateRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PurchaseOrderSummaryDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetQueryable()
                .Include(o => o.Supplier)
                .Include(o => o.PurchaseItems)
                .OrderByDescending(o => o.PurchaseDate)
                .ToListAsync();

            return orders.Select(o => new PurchaseOrderSummaryDto
            {
                PurchaseOrderId = o.PurchaseOrderId,
                OrderNumber = o.OrderNumber,
                SupplierName = o.Supplier?.SupplierName ?? "Unknown",
                PurchaseDate = o.PurchaseDate,
                GrandTotal = o.GrandTotal,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                ItemCount = o.PurchaseItems?.Count ?? 0,
                TotalQuantity = o.PurchaseItems?.Sum(i => i.Quantity) ?? 0
            });
        }

        public async Task<PurchaseOrderDto?> GetByIdAsync(int id)
        {
            var order = await _orderRepository.GetQueryable()
                .Include(o => o.Supplier)
                .Include(o => o.PurchaseItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.PurchaseOrderId == id);

            if (order == null) return null;

            var orderDto = _mapper.Map<PurchaseOrderDto>(order);
            orderDto.SupplierName = order.Supplier?.SupplierName;

            if (order.PurchaseItems != null)
            {
                foreach (var item in orderDto.PurchaseItems)
                {
                    var purchaseItem = order.PurchaseItems.FirstOrDefault(i => i.PurchaseItemId == item.PurchaseItemId);
                    item.ProductName = purchaseItem?.Product?.ProductName;
                }
            }

            return orderDto;
        }

        public async Task<int> CreateAsync(CreatePurchaseOrderDto dto)
        {
            // Validate supplier exists
            var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId);
            if (supplier == null)
                throw new Exception($"Supplier with ID {dto.SupplierId} not found");

            if (dto.PurchaseItems == null || !dto.PurchaseItems.Any())
                throw new Exception("At least one item is required");

            // Generate order number
            var orderNumber = await GenerateOrderNumberAsync();

            // Create order
            var order = new PurchaseOrder
            {
                OrderNumber = orderNumber,
                SupplierId = dto.SupplierId,
                PurchaseDate = dto.PurchaseDate,
                Notes = dto.Notes,
                Status = "Draft",
                PaymentStatus = "Pending",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            decimal totalAmount = 0;
            decimal totalDiscount = 0;
            decimal totalTax = 0;

            // Process each item
            foreach (var itemDto in dto.PurchaseItems)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {itemDto.ProductId} not found");

                // Get tax rate for the product
                decimal taxPercentage = 0;
                if (product.TaxRateId.HasValue)
                {
                    var taxRate = await _taxRateRepository.GetByIdAsync(product.TaxRateId.Value);
                    taxPercentage = taxRate?.Rate ?? 0;
                }

                // Calculate costs
                decimal itemDiscount = itemDto.DiscountAmount ?? 0;
                decimal itemSubtotal = itemDto.UnitCost * itemDto.Quantity;
                decimal itemTax = (itemSubtotal - itemDiscount) * (taxPercentage / 100);
                decimal itemTotal = itemSubtotal - itemDiscount + itemTax;

                // Create purchase item
                var item = new PurchaseItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitCost = itemDto.UnitCost,
                    DiscountAmount = itemDiscount,
                    TaxAmount = itemTax,
                    TotalCost = itemTotal,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                order.PurchaseItems.Add(item);

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
            return order.PurchaseOrderId;
        }

        public async Task UpdateAsync(int id, UpdatePurchaseOrderDto dto)
        {
            if (id != dto.PurchaseOrderId)
                throw new Exception("ID mismatch");

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new Exception("Purchase order not found");

            // Only allow updates to certain fields
            order.Notes = dto.Notes ?? order.Notes;
            order.Status = dto.Status;
            order.PaymentStatus = dto.PaymentStatus;
            order.IsActive = dto.IsActive;
            order.ModifiedDate = DateTime.UtcNow;

            _orderRepository.Update(order);
        }

        public async Task UpdateStatusAsync(int id, UpdatePurchaseOrderStatusDto dto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new Exception("Purchase order not found");

            // Validate status transition
            if (order.Status == "Received" || order.Status == "Cancelled")
                throw new Exception($"Cannot change status of {order.Status} orders");

            order.Status = dto.Status;
            order.ModifiedDate = DateTime.UtcNow;

            _orderRepository.Update(order);
        }

        public async Task UpdatePaymentStatusAsync(int id, UpdatePurchasePaymentStatusDto dto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new Exception("Purchase order not found");

            order.PaymentStatus = dto.PaymentStatus;
            order.ModifiedDate = DateTime.UtcNow;

            _orderRepository.Update(order);
        }

        public async Task ReceiveOrderAsync(int id, ReceivePurchaseOrderDto dto)
        {
            var order = await _orderRepository.GetQueryable()
                .Include(o => o.PurchaseItems)
                .FirstOrDefaultAsync(o => o.PurchaseOrderId == id);

            if (order == null)
                throw new Exception("Purchase order not found");

            if (order.Status != "Approved" && order.Status != "Shipped")
                throw new Exception("Only approved or shipped orders can be received");

            // Update product stock
            foreach (var item in order.PurchaseItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    _productRepository.Update(product);
                }
            }

            // Update order status
            order.Status = "Received";
            order.Notes = dto.ReceivedNotes ?? order.Notes;
            order.ModifiedDate = DateTime.UtcNow;

            _orderRepository.Update(order);
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _orderRepository.GetQueryable()
                .Include(o => o.PurchaseItems)
                .FirstOrDefaultAsync(o => o.PurchaseOrderId == id);

            if (order == null)
                throw new Exception("Purchase order not found");

            // Check if order can be deleted
            if (order.Status != "Draft" && order.Status != "Cancelled")
                throw new Exception("Only draft or cancelled orders can be deleted");

            _orderRepository.Delete(order);
        }

        public async Task<IEnumerable<PurchaseOrderSummaryDto>> GetOrdersBySupplierAsync(int supplierId)
        {
            var orders = await _orderRepository.GetQueryable()
                .Include(o => o.Supplier)
                .Include(o => o.PurchaseItems)
                .Where(o => o.SupplierId == supplierId)
                .OrderByDescending(o => o.PurchaseDate)
                .ToListAsync();

            return orders.Select(o => new PurchaseOrderSummaryDto
            {
                PurchaseOrderId = o.PurchaseOrderId,
                OrderNumber = o.OrderNumber,
                SupplierName = o.Supplier?.SupplierName ?? "Unknown",
                PurchaseDate = o.PurchaseDate,
                GrandTotal = o.GrandTotal,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                ItemCount = o.PurchaseItems?.Count ?? 0,
                TotalQuantity = o.PurchaseItems?.Sum(i => i.Quantity) ?? 0
            });
        }

        public async Task<IEnumerable<PurchaseOrderSummaryDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetQueryable()
                .Include(o => o.Supplier)
                .Include(o => o.PurchaseItems)
                .Where(o => o.PurchaseDate >= startDate && o.PurchaseDate <= endDate)
                .OrderByDescending(o => o.PurchaseDate)
                .ToListAsync();

            return orders.Select(o => new PurchaseOrderSummaryDto
            {
                PurchaseOrderId = o.PurchaseOrderId,
                OrderNumber = o.OrderNumber,
                SupplierName = o.Supplier?.SupplierName ?? "Unknown",
                PurchaseDate = o.PurchaseDate,
                GrandTotal = o.GrandTotal,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                ItemCount = o.PurchaseItems?.Count ?? 0,
                TotalQuantity = o.PurchaseItems?.Sum(i => i.Quantity) ?? 0
            });
        }

        public async Task<IEnumerable<PurchaseOrderSummaryDto>> GetOrdersByStatusAsync(string status)
        {
            var orders = await _orderRepository.GetQueryable()
                .Include(o => o.Supplier)
                .Include(o => o.PurchaseItems)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.PurchaseDate)
                .ToListAsync();

            return orders.Select(o => new PurchaseOrderSummaryDto
            {
                PurchaseOrderId = o.PurchaseOrderId,
                OrderNumber = o.OrderNumber,
                SupplierName = o.Supplier?.SupplierName ?? "Unknown",
                PurchaseDate = o.PurchaseDate,
                GrandTotal = o.GrandTotal,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                ItemCount = o.PurchaseItems?.Count ?? 0,
                TotalQuantity = o.PurchaseItems?.Sum(i => i.Quantity) ?? 0
            });
        }

        public async Task<IEnumerable<PurchaseOrderSummaryDto>> GetOrdersByPaymentStatusAsync(string paymentStatus)
        {
            var orders = await _orderRepository.GetQueryable()
                .Include(o => o.Supplier)
                .Include(o => o.PurchaseItems)
                .Where(o => o.PaymentStatus == paymentStatus)
                .OrderByDescending(o => o.PurchaseDate)
                .ToListAsync();

            return orders.Select(o => new PurchaseOrderSummaryDto
            {
                PurchaseOrderId = o.PurchaseOrderId,
                OrderNumber = o.OrderNumber,
                SupplierName = o.Supplier?.SupplierName ?? "Unknown",
                PurchaseDate = o.PurchaseDate,
                GrandTotal = o.GrandTotal,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                ItemCount = o.PurchaseItems?.Count ?? 0,
                TotalQuantity = o.PurchaseItems?.Sum(i => i.Quantity) ?? 0
            });
        }

        public async Task<IEnumerable<PurchaseOrderSummaryDto>> SearchOrdersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower().Trim();

            var orders = await _orderRepository.GetQueryable()
                .Include(o => o.Supplier)
                .Include(o => o.PurchaseItems)
                .Where(o =>
                    o.OrderNumber.ToLower().Contains(searchTerm) ||
                    (o.Supplier != null && o.Supplier.SupplierName.ToLower().Contains(searchTerm)) ||
                    o.Status.ToLower().Contains(searchTerm) ||
                    o.PaymentStatus.ToLower().Contains(searchTerm))
                .OrderByDescending(o => o.PurchaseDate)
                .ToListAsync();

            return orders.Select(o => new PurchaseOrderSummaryDto
            {
                PurchaseOrderId = o.PurchaseOrderId,
                OrderNumber = o.OrderNumber,
                SupplierName = o.Supplier?.SupplierName ?? "Unknown",
                PurchaseDate = o.PurchaseDate,
                GrandTotal = o.GrandTotal,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                ItemCount = o.PurchaseItems?.Count ?? 0,
                TotalQuantity = o.PurchaseItems?.Sum(i => i.Quantity) ?? 0
            });
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _orderRepository.GetQueryable().AnyAsync(o => o.PurchaseOrderId == id);
        }

        public async Task<bool> CanCancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            // Can cancel if draft, submitted, or approved
            return order.Status == "Draft" || order.Status == "Submitted" || order.Status == "Approved";
        }

        public async Task<bool> CanReceiveOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            // Can receive if approved or shipped
            return order.Status == "Approved" || order.Status == "Shipped";
        }

        public async Task<decimal> CalculateOrderTotalAsync(int id)
        {
            var order = await _orderRepository.GetQueryable()
                .Include(o => o.PurchaseItems)
                .FirstOrDefaultAsync(o => o.PurchaseOrderId == id);

            if (order == null)
                throw new Exception("Purchase order not found");

            return order.PurchaseItems?.Sum(i => i.TotalCost) ?? 0;
        }

        private async Task<string> GenerateOrderNumberAsync()
        {
            var today = DateTime.Today;
            var prefix = "PO-" + today.ToString("yyyyMMdd") + "-";

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
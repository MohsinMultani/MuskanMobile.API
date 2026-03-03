using MuskanMobile.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuskanMobile.Application.Interfaces
{
    public interface IPurchaseOrderService
    {
        // Basic CRUD
        Task<IEnumerable<PurchaseOrderSummaryDto>> GetAllAsync();
        Task<PurchaseOrderDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreatePurchaseOrderDto dto);
        Task UpdateAsync(int id, UpdatePurchaseOrderDto dto);
        Task DeleteAsync(int id);

        // Status updates
        Task UpdateStatusAsync(int id, UpdatePurchaseOrderStatusDto dto);
        Task UpdatePaymentStatusAsync(int id, UpdatePurchasePaymentStatusDto dto);
        Task ReceiveOrderAsync(int id, ReceivePurchaseOrderDto dto);

        // Filtering and search
        Task<IEnumerable<PurchaseOrderSummaryDto>> GetOrdersBySupplierAsync(int supplierId);
        Task<IEnumerable<PurchaseOrderSummaryDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PurchaseOrderSummaryDto>> GetOrdersByStatusAsync(string status);
        Task<IEnumerable<PurchaseOrderSummaryDto>> GetOrdersByPaymentStatusAsync(string paymentStatus);
        Task<IEnumerable<PurchaseOrderSummaryDto>> SearchOrdersAsync(string searchTerm);

        // Validation and utilities
        Task<bool> ExistsAsync(int id);
        Task<bool> CanCancelOrderAsync(int id);
        Task<bool> CanReceiveOrderAsync(int id);
        Task<decimal> CalculateOrderTotalAsync(int id);
    }
}
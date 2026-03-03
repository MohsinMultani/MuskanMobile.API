using MuskanMobile.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuskanMobile.Application.Interfaces
{
    public interface ISalesOrderService
    {
        // Basic CRUD
        Task<IEnumerable<SalesOrderSummaryDto>> GetAllAsync();
        Task<SalesOrderDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateSalesOrderDto dto);
        Task UpdateAsync(int id, UpdateSalesOrderDto dto);
        Task DeleteAsync(int id);

        // Status updates
        Task UpdateStatusAsync(int id, UpdateSalesOrderStatusDto dto);
        Task UpdatePaymentStatusAsync(int id, UpdateSalesPaymentStatusDto dto);

        // Filtering and search
        Task<IEnumerable<SalesOrderSummaryDto>> GetOrdersByCustomerAsync(int customerId);
        Task<IEnumerable<SalesOrderSummaryDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<SalesOrderSummaryDto>> GetOrdersByStatusAsync(string status);
        Task<IEnumerable<SalesOrderSummaryDto>> GetOrdersByPaymentStatusAsync(string paymentStatus);
        Task<IEnumerable<SalesOrderSummaryDto>> SearchOrdersAsync(string searchTerm);

        // Validation and utilities
        Task<bool> ExistsAsync(int id);
        Task<bool> CanCancelOrderAsync(int id);
        Task<decimal> CalculateOrderTotalAsync(int id);
        Task<bool> ProcessPaymentAsync(int id, decimal amount);
    }
}
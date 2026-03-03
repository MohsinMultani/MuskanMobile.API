using Microsoft.AspNetCore.Mvc;
using MuskanMobile.Application.DTOs;
using MuskanMobile.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace MuskanMobile.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ISalesOrderService _service;

        public SalesOrdersController(ISalesOrderService service)
        {
            _service = service;
        }

        // GET: api/salesorders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _service.GetAllAsync();
            return Ok(orders);
        }

        // GET: api/salesorders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
                return NotFound($"Sales order with ID {id} not found");

            return Ok(order);
        }

        // GET: api/salesorders/customer/5
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var orders = await _service.GetOrdersByCustomerAsync(customerId);
            return Ok(orders);
        }

        // GET: api/salesorders/status/Pending
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var orders = await _service.GetOrdersByStatusAsync(status);
            return Ok(orders);
        }

        // GET: api/salesorders/payment/Pending
        [HttpGet("payment/{paymentStatus}")]
        public async Task<IActionResult> GetByPaymentStatus(string paymentStatus)
        {
            var orders = await _service.GetOrdersByPaymentStatusAsync(paymentStatus);
            return Ok(orders);
        }

        // GET: api/salesorders/daterange?start=2024-01-01&end=2024-03-01
        [HttpGet("daterange")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var orders = await _service.GetOrdersByDateRangeAsync(start, end);
            return Ok(orders);
        }

        // GET: api/salesorders/search?term=SO-2024
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var orders = await _service.SearchOrdersAsync(term);
            return Ok(orders);
        }

        // GET: api/salesorders/5/cancancel
        [HttpGet("{id}/cancancel")]
        public async Task<IActionResult> CanCancel(int id)
        {
            var canCancel = await _service.CanCancelOrderAsync(id);
            return Ok(new { orderId = id, canCancel });
        }

        // GET: api/salesorders/5/total
        [HttpGet("{id}/total")]
        public async Task<IActionResult> GetTotal(int id)
        {
            try
            {
                var total = await _service.CalculateOrderTotalAsync(id);
                return Ok(new { orderId = id, total });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/salesorders
        [HttpPost]
        public async Task<IActionResult> Create(CreateSalesOrderDto dto)
        {
            try
            {
                var id = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT: api/salesorders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSalesOrderDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PATCH: api/salesorders/5/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateSalesOrderStatusDto dto)
        {
            try
            {
                await _service.UpdateStatusAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PATCH: api/salesorders/5/payment
        [HttpPatch("{id}/payment")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, UpdateSalesPaymentStatusDto dto)
        {
            try
            {
                await _service.UpdatePaymentStatusAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/salesorders/5/processpayment?amount=50000
        [HttpPost("{id}/processpayment")]
        public async Task<IActionResult> ProcessPayment(int id, [FromQuery] decimal amount)
        {
            try
            {
                await _service.ProcessPaymentAsync(id, amount);
                return Ok(new { message = "Payment processed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/salesorders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // HEAD: api/salesorders/5
        [HttpHead("{id}")]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _service.ExistsAsync(id);
            return exists ? Ok() : NotFound();
        }
    }
}
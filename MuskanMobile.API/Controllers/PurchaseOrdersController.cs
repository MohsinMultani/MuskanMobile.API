using Microsoft.AspNetCore.Mvc;
using MuskanMobile.Application.DTOs;
using MuskanMobile.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace MuskanMobile.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _service;

        public PurchaseOrdersController(IPurchaseOrderService service)
        {
            _service = service;
        }

        // GET: api/purchaseorders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _service.GetAllAsync();
            return Ok(orders);
        }

        // GET: api/purchaseorders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
                return NotFound($"Purchase order with ID {id} not found");

            return Ok(order);
        }

        // GET: api/purchaseorders/supplier/5
        [HttpGet("supplier/{supplierId}")]
        public async Task<IActionResult> GetBySupplier(int supplierId)
        {
            var orders = await _service.GetOrdersBySupplierAsync(supplierId);
            return Ok(orders);
        }

        // GET: api/purchaseorders/status/Draft
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var orders = await _service.GetOrdersByStatusAsync(status);
            return Ok(orders);
        }

        // GET: api/purchaseorders/payment/Pending
        [HttpGet("payment/{paymentStatus}")]
        public async Task<IActionResult> GetByPaymentStatus(string paymentStatus)
        {
            var orders = await _service.GetOrdersByPaymentStatusAsync(paymentStatus);
            return Ok(orders);
        }

        // GET: api/purchaseorders/daterange?start=2024-01-01&end=2024-03-01
        [HttpGet("daterange")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var orders = await _service.GetOrdersByDateRangeAsync(start, end);
            return Ok(orders);
        }

        // GET: api/purchaseorders/search?term=PO-2024
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var orders = await _service.SearchOrdersAsync(term);
            return Ok(orders);
        }

        // GET: api/purchaseorders/5/cancancel
        [HttpGet("{id}/cancancel")]
        public async Task<IActionResult> CanCancel(int id)
        {
            var canCancel = await _service.CanCancelOrderAsync(id);
            return Ok(new { purchaseOrderId = id, canCancel });
        }

        // GET: api/purchaseorders/5/canreceive
        [HttpGet("{id}/canreceive")]
        public async Task<IActionResult> CanReceive(int id)
        {
            var canReceive = await _service.CanReceiveOrderAsync(id);
            return Ok(new { purchaseOrderId = id, canReceive });
        }

        // GET: api/purchaseorders/5/total
        [HttpGet("{id}/total")]
        public async Task<IActionResult> GetTotal(int id)
        {
            try
            {
                var total = await _service.CalculateOrderTotalAsync(id);
                return Ok(new { purchaseOrderId = id, total });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/purchaseorders
        [HttpPost]
        public async Task<IActionResult> Create(CreatePurchaseOrderDto dto)
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

        // PUT: api/purchaseorders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePurchaseOrderDto dto)
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

        // PATCH: api/purchaseorders/5/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdatePurchaseOrderStatusDto dto)
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

        // PATCH: api/purchaseorders/5/payment
        [HttpPatch("{id}/payment")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, UpdatePurchasePaymentStatusDto dto)
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

        // POST: api/purchaseorders/5/receive
        [HttpPost("{id}/receive")]
        public async Task<IActionResult> ReceiveOrder(int id, ReceivePurchaseOrderDto dto)
        {
            try
            {
                await _service.ReceiveOrderAsync(id, dto);
                return Ok(new { message = "Order received successfully. Stock updated." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/purchaseorders/5
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

        // HEAD: api/purchaseorders/5
        [HttpHead("{id}")]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _service.ExistsAsync(id);
            return exists ? Ok() : NotFound();
        }
    }
}
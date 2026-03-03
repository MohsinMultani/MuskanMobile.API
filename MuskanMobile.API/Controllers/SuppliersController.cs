using Microsoft.AspNetCore.Mvc;
using MuskanMobile.Application.DTOs;
using MuskanMobile.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace MuskanMobile.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SuppliersController(ISupplierService service)
        {
            _service = service;
        }

        // GET: api/suppliers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _service.GetAllAsync();
            return Ok(suppliers);
        }

        // GET: api/suppliers/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var suppliers = await _service.GetActiveSuppliersAsync();
            return Ok(suppliers);
        }

        // GET: api/suppliers/dropdown
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var suppliers = await _service.GetSupplierDropdownAsync();
            return Ok(suppliers);
        }

        // GET: api/suppliers/search?term=apple
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var suppliers = await _service.SearchSuppliersAsync(term);
            return Ok(suppliers);
        }

        // GET: api/suppliers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _service.GetByIdAsync(id);
            if (supplier == null)
                return NotFound($"Supplier with ID {id} not found");

            return Ok(supplier);
        }

        // GET: api/suppliers/5/hasproducts
        [HttpGet("{id}/hasproducts")]
        public async Task<IActionResult> HasProducts(int id)
        {
            var exists = await _service.ExistsAsync(id);
            if (!exists)
                return NotFound($"Supplier with ID {id} not found");

            var hasProducts = await _service.HasProductsAsync(id);
            return Ok(new { supplierId = id, hasProducts });
        }

        // POST: api/suppliers
        [HttpPost]
        public async Task<IActionResult> Create(CreateSupplierDto dto)
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

        // PUT: api/suppliers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSupplierDto dto)
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

        // DELETE: api/suppliers/5
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

        // HEAD: api/suppliers/5 (check if exists)
        [HttpHead("{id}")]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _service.ExistsAsync(id);
            return exists ? Ok() : NotFound();
        }
    }
}
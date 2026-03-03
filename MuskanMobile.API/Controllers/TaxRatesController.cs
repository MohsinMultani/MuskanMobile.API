using Microsoft.AspNetCore.Mvc;
using MuskanMobile.Application.DTOs;
using MuskanMobile.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace MuskanMobile.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxRatesController : ControllerBase
    {
        private readonly ITaxRateService _service;

        public TaxRatesController(ITaxRateService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var taxRates = await _service.GetAllAsync();
            return Ok(taxRates);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var taxRates = await _service.GetActiveTaxRatesAsync();
            return Ok(taxRates);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var taxRates = await _service.GetTaxRateDropdownAsync();
            return Ok(taxRates);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var taxRates = await _service.SearchTaxRatesAsync(term);
            return Ok(taxRates);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var taxRate = await _service.GetByIdAsync(id);
            if (taxRate == null)
                return NotFound($"Tax rate with ID {id} not found");

            return Ok(taxRate);
        }

        [HttpGet("{id}/hasproducts")]
        public async Task<IActionResult> HasProducts(int id)
        {
            var exists = await _service.ExistsAsync(id);
            if (!exists)
                return NotFound($"Tax rate with ID {id} not found");

            var hasProducts = await _service.HasProductsAsync(id);
            return Ok(new { taxRateId = id, hasProducts });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaxRateDto dto)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTaxRateDto dto)
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

        [HttpGet("checkname")]
        public async Task<IActionResult> CheckNameUnique([FromQuery] string name, [FromQuery] int? excludeId = null)
        {
            var isUnique = await _service.IsTaxNameUniqueAsync(name, excludeId);
            return Ok(new { name, isUnique });
        }

        [HttpHead("{id}")]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _service.ExistsAsync(id);
            return exists ? Ok() : NotFound();
        }
    }
}
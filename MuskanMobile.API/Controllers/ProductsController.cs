//using Microsoft.AspNetCore.Mvc;
//using MuskanMobile.Application.DTOs;
//using MuskanMobile.Application.Interfaces;
//using System.Threading.Tasks;

//[ApiController]
//[Route("api/[controller]")]
//public class ProductsController : ControllerBase
//{
//    private readonly IProductService _service;

//    public ProductsController(IProductService service)
//    {
//        _service = service;
//    }

//    [HttpGet]
//    public async Task<IActionResult> Get()
//        => Ok(await _service.GetAllAsync());

//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        var result = await _service.GetByIdAsync(id);
//        if (result == null) return NotFound();
//        return Ok(result);
//    }

//    [HttpPost]
//    public async Task<IActionResult> Create(CreateProductDto dto)
//        => Ok(await _service.CreateAsync(dto));

//    [HttpPut("{id}")]
//    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
//    {
//        await _service.UpdateAsync(id, dto);
//        return NoContent();
//    }

//    [HttpDelete("{id}")]
//    public async Task<IActionResult> Delete(int id)
//    {
//        await _service.DeleteAsync(id);
//        return NoContent();
//    }


//}


using Microsoft.AspNetCore.Mvc;
using MuskanMobile.Application.DTOs;
using MuskanMobile.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace MuskanMobile.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        // GET: api/products/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var products = await _service.GetActiveProductsAsync();
            return Ok(products);
        }

        // GET: api/products/dropdown
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var products = await _service.GetProductDropdownAsync();
            return Ok(products);
        }

        // GET: api/products/search?term=iphone
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var products = await _service.SearchProductsAsync(term);
            return Ok(products);
        }

        // POST: api/products/filter
        [HttpPost("filter")]
        public async Task<IActionResult> Filter(ProductFilterDto filter)
        {
            var products = await _service.FilterProductsAsync(filter);
            return Ok(products);
        }

        // GET: api/products/bycategory/5
        [HttpGet("bycategory/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _service.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        // GET: api/products/bysupplier/5
        [HttpGet("bysupplier/{supplierId}")]
        public async Task<IActionResult> GetBySupplier(int supplierId)
        {
            var products = await _service.GetProductsBySupplierAsync(supplierId);
            return Ok(products);
        }

        // GET: api/products/bytaxrate/5
        [HttpGet("bytaxrate/{taxRateId}")]
        public async Task<IActionResult> GetByTaxRate(int taxRateId)
        {
            var products = await _service.GetProductsByTaxRateAsync(taxRateId);
            return Ok(products);
        }

        // GET: api/products/lowstock?threshold=5
        [HttpGet("lowstock")]
        public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 10)
        {
            var products = await _service.GetLowStockProductsAsync(threshold);
            return Ok(products);
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null)
                return NotFound($"Product with ID {id} not found");

            return Ok(product);
        }

        // GET: api/products/5/stock
        [HttpGet("{id}/stock")]
        public async Task<IActionResult> GetStock(int id)
        {
            var stock = await _service.GetProductStockAsync(id);
            if (stock == null)
                return NotFound($"Product with ID {id} not found");

            return Ok(stock);
        }

        // GET: api/products/5/pricewithtax
        [HttpGet("{id}/pricewithtax")]
        public async Task<IActionResult> GetPriceWithTax(int id)
        {
            try
            {
                var price = await _service.GetProductPriceWithTaxAsync(id);
                return Ok(new { productId = id, priceWithTax = price });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
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

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
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

        // PATCH: api/products/5/stock
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] StockUpdateDto stockUpdate)
        {
            try
            {
                var result = await _service.UpdateStockAsync(id, stockUpdate.Quantity, stockUpdate.IsAddition);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/products/5
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

        // GET: api/products/checksku?sku=ABC123&excludeId=1
        [HttpGet("checksku")]
        public async Task<IActionResult> CheckSkuUnique([FromQuery] string sku, [FromQuery] int? excludeId = null)
        {
            var isUnique = await _service.IsSkuUniqueAsync(sku, excludeId);
            return Ok(new { sku, isUnique });
        }

        // HEAD: api/products/5
        [HttpHead("{id}")]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _service.ExistsAsync(id);
            return exists ? Ok() : NotFound();
        }
    }

    public class StockUpdateDto
    {
        public int Quantity { get; set; }
        public bool IsAddition { get; set; } // true = add, false = remove
    }
}
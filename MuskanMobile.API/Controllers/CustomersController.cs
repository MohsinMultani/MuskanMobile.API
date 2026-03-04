using Humanizer.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MuskanMobile.Application.DTOs;
using MuskanMobile.Application.DTOs.Customer;
using MuskanMobile.Application.Interfaces;
using MuskanMobile.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuskanMobile.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;
        private readonly IConfiguration _configuration;

        public CustomersController(ICustomerService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpGet("debug-connection")]
        public IActionResult DebugConnection()
        {
            var results = new Dictionary<string, string>();

            // Check environment variables
            results.Add("SQLAZURECONNSTR_DefaultConnection",
                Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection") != null ? "Found" : "Not Found");

            results.Add("ConnectionStrings__DefaultConnection",
                Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") != null ? "Found" : "Not Found");

            results.Add("DEFAULT_CONNECTION",
                Environment.GetEnvironmentVariable("DEFAULT_CONNECTION") != null ? "Found" : "Not Found");

            // Get the actual connection string being used (masked)
            var connString = _configuration.GetConnectionString("DefaultConnection");
            if (connString != null)
            {
                var masked = System.Text.RegularExpressions.Regex.Replace(
                    connString, "Password=.*?;", "Password=*****;");
                results.Add("Current Connection String", masked);
            }

            return Ok(results);
        }

        [HttpGet("debug-finalllllll")]
        public async Task<IActionResult> DebugFinal()
        {
            var results = new Dictionary<string, object>();

            try
            {
                using (var connection = new Microsoft.Data.SqlClient.SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    results.Add("Connection", "OK");

                    // Test Customers table
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "SELECT COUNT(*) FROM [Muskan].[Customers]";
                    var count = await cmd.ExecuteScalarAsync();
                    results.Add("Customers Count", count);

                    // Get current user
                    cmd.CommandText = "SELECT USER_NAME()";
                    var user = await cmd.ExecuteScalarAsync();
                    results.Add("Database User", user);
                }
            }
            catch (Exception ex)
            {
                results.Add("Error", ex.Message);
            }

            return Ok(results);
        }

        [HttpGet("debug-dbcontext")]
        public async Task<IActionResult> DebugDbContext()
        {
            try
            {
                // Force a new context instance
                using (var scope = HttpContext.RequestServices.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<MuskanMobileDbContext>();
                    var count = await context.Customers.CountAsync();
                    return Ok(new
                    {
                        method = "Using DbContext directly",
                        customerCount = count,
                        success = true
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    method = "DbContext failed",
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }


        [HttpGet("debug-compare")]
        public async Task<IActionResult> DebugCompare()
        {
            var results = new Dictionary<string, object>();

            // 1. Get connection string from configuration
            var configConn = _configuration.GetConnectionString("DefaultConnection");
            results.Add("Config Connection", configConn != null ? "Found" : "Not Found");

            // 2. Get connection string from environment (what DbContext might use)
            var envConn = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection");
            results.Add("Env Connection", envConn != null ? "Found" : "Not Found");

            // 3. Try to get DbContext's connection string via reflection
            try
            {
                using (var scope = HttpContext.RequestServices.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<MuskanMobileDbContext>();
                    var dbConn = context.Database.GetDbConnection();
                    results.Add("DbContext Connection String", dbConn.ConnectionString?.Substring(0, 50) + "...");
                }
            }
            catch (Exception ex)
            {
                results.Add("DbContext Error", ex.Message);
            }

            return Ok(results);
        }

        // GET: api/customers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _service.GetAllAsync();
            return Ok(customers);
        }

        // GET: api/customers/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var customers = await _service.GetActiveCustomersAsync();
            return Ok(customers);
        }

        // GET: api/customers/dropdown
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var customers = await _service.GetCustomerDropdownAsync();
            return Ok(customers);
        }

        // GET: api/customers/search?term=john
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var customers = await _service.SearchCustomersAsync(term);
            return Ok(customers);
        }

        // GET: api/customers/bycity?city=Mumbai
        [HttpGet("bycity")]
        public async Task<IActionResult> GetByCity([FromQuery] string city)
        {
            var customers = await _service.GetCustomersByCityAsync(city);
            return Ok(customers);
        }

        // GET: api/customers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _service.GetByIdAsync(id);
            if (customer == null)
                return NotFound($"Customer with ID {id} not found");

            return Ok(customer);
        }

        // GET: api/customers/5/hasorders
        [HttpGet("{id}/hasorders")]
        public async Task<IActionResult> HasOrders(int id)
        {
            var exists = await _service.ExistsAsync(id);
            if (!exists)
                return NotFound($"Customer with ID {id} not found");

            var hasOrders = await _service.HasSalesOrdersAsync(id);
            return Ok(new { customerId = id, hasSalesOrders = hasOrders });
        }

        // POST: api/customers
        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDto dto)
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

        // PUT: api/customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCustomerDto dto)
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

        // DELETE: api/customers/5
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

        // GET: api/customers/checkemail?email=test@test.com&excludeId=1
        [HttpGet("checkemail")]
        public async Task<IActionResult> CheckEmailUnique([FromQuery] string email, [FromQuery] int? excludeId = null)
        {
            var isUnique = await _service.IsEmailUniqueAsync(email, excludeId);
            return Ok(new { email, isUnique });
        }

        // GET: api/customers/checkphone?phone=1234567890&excludeId=1
        [HttpGet("checkphone")]
        public async Task<IActionResult> CheckPhoneUnique([FromQuery] string phone, [FromQuery] int? excludeId = null)
        {
            var isUnique = await _service.IsPhoneUniqueAsync(phone, excludeId);
            return Ok(new { phone, isUnique });
        }

        // HEAD: api/customers/5
        [HttpHead("{id}")]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _service.ExistsAsync(id);
            return exists ? Ok() : NotFound();
        }
    }
}
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Configuration;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System;

//namespace MuskanMobile.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class DiagnosticController : ControllerBase
//    {
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuskanMobile.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // Constructor with dependency injection
        public DiagnosticController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("test-db")]
        public async Task<IActionResult> TestDatabaseConnection()
        {
            var results = new List<string>();

            try
            {
                // Get connection string
                var connString = _configuration.GetConnectionString("DefaultConnection");
                results.Add($"✅ Connection string found: {connString != null}");

                if (string.IsNullOrEmpty(connString))
                {
                    results.Add("❌ ERROR: Connection string 'DefaultConnection' is missing!");
                    return Ok(results);
                }

                // Mask password for logging
                var displayString = connString;
                if (displayString.Contains("Password="))
                {
                    var parts = displayString.Split(';');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (parts[i].StartsWith("Password=", StringComparison.OrdinalIgnoreCase))
                        {
                            parts[i] = "Password=*****";
                        }
                    }
                    displayString = string.Join(";", parts);
                }
                results.Add($"📋 Connection string: {displayString}");

                // Test connection
                results.Add($"🔄 Attempting to connect to database...");

                using (var connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();
                    results.Add($"✅ SUCCESS: Connected to database!");

                    // Get database info
                    using (var command = new SqlCommand("SELECT DB_NAME() AS DatabaseName, @@VERSION AS Version", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var dbName = reader["DatabaseName"].ToString();
                                var version = reader["Version"].ToString();
                                results.Add($"📊 Database name: {dbName}");
                                results.Add($"ℹ️ SQL Version: {version?.Substring(0, Math.Min(50, version.Length))}...");
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (SqlException ex)
            {
                results.Add($"❌ SQL ERROR: {ex.Message}");
                results.Add($"🔢 Error Number: {ex.Number}");

                // Specific error code explanations
                switch (ex.Number)
                {
                    case 40615:
                        results.Add($"🔍 MEANING: Firewall rule missing for this IP");
                        break;
                    case 18456:
                        results.Add($"🔍 MEANING: Login failed - check username/password");
                        break;
                    case 53:
                    case 26:
                        results.Add($"🔍 MEANING: Network issue - can't find server");
                        break;
                }
            }
            catch (Exception ex)
            {
                results.Add($"❌ GENERAL ERROR: {ex.Message}");
            }

            return Ok(results);
        }

        [HttpGet("test-simple")]
        public IActionResult SimpleTest()
        {
            return Ok(new
            {
                message = "Diagnostic endpoint is working!",
                timestamp = DateTime.Now,
                hasConnectionString = _configuration.GetConnectionString("DefaultConnection") != null
            });
        }
    }
}
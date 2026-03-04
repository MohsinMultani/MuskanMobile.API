//using Microsoft.EntityFrameworkCore;
//using MuskanMobile.Infrastructure.Repositories;
//using MuskanMobile.Application.Interfaces;
//using MuskanMobile.Infrastructure.Data;
//using MuskanMobile.Application.Services;
//using MuskanMobile.Infrastructure.Interceptors;
//using Microsoft.AspNetCore.HttpOverrides;
//using Microsoft.AspNetCore.Builder;
//using System;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Configuration;


//var builder = WebApplication.CreateBuilder(args);

//// Get connection string from configuration
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//if (string.IsNullOrEmpty(connectionString))
//{
//    throw new InvalidOperationException("Database connection string 'DefaultConnection' not found.");
//}

//// Register DbContext
//builder.Services.AddDbContext<MuskanMobileDbContext>(options =>
//{
//    options.UseSqlServer(
//        connectionString,
//        sqlOptions => sqlOptions.CommandTimeout(60)
//    );
//    options.AddInterceptors(new AuditableEntityInterceptor());
//});

//// Register repositories
//builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

//// Register services
//builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddScoped<ICategoryService, CategoryService>();
//builder.Services.AddScoped<ISupplierService, SupplierService>();
//builder.Services.AddScoped<ITaxRateService, TaxRateService>();
//builder.Services.AddScoped<ICustomerService, CustomerService>();
//builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();
//builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

//builder.Services.AddSingleton<ITelemetryService, TelemetryService>();

//// Register AutoMapper
//builder.Services.AddAutoMapper(typeof(MappingProfile));

//// Register controllers and Swagger
//builder.Services.AddControllers();
//builder.Services.AddApplicationInsightsTelemetry();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}
//else
//{
//    app.UseDeveloperExceptionPage(); // Remove this after testing
//}

//// Forwarded headers for Azure
//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
//    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
//});

//// Swagger
//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MuskanMobile API V1");
//    c.RoutePrefix = "swagger";
//});

//app.UseHttpsRedirection();
//app.MapControllers();

//app.Run();



using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using MuskanMobile.Infrastructure.Repositories;
using MuskanMobile.Application.Interfaces;
using MuskanMobile.Infrastructure.Data;
using MuskanMobile.Application.Services;
using MuskanMobile.Infrastructure.Interceptors;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// ========== KEY VAULT CONFIGURATION ==========
string connectionString = "";

// Check if we're in Azure production
if (builder.Environment.IsProduction())
{
    try
    {
        // For Azure App Service with Managed Identity
        var keyVaultEndpoint = new Uri("https://muskanmobile-kv.vault.azure.net/");
        var secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());

        // Retrieve the connection string from Key Vault
        KeyVaultSecret secret = await secretClient.GetSecretAsync("sql-connection-string");
        connectionString = secret.Value;

        Console.WriteLine("✅ Successfully retrieved connection string from Key Vault");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Failed to retrieve from Key Vault: {ex.Message}");
        throw; // Fail fast in production - don't start without DB connection
    }
}
else
{
    // Local development - use appsettings.json
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("✅ Using local connection string for development");
}

// Register DbContext
builder.Services.AddDbContext<MuskanMobileDbContext>(options =>
{
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.CommandTimeout(60)
    );
    options.AddInterceptors(new AuditableEntityInterceptor());
});

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ITaxRateService, TaxRateService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddSingleton<ITelemetryService, TelemetryService>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register controllers, Application Insights, and Swagger
builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseDeveloperExceptionPage(); // Remove after testing
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MuskanMobile API V1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.MapControllers();

app.Run();